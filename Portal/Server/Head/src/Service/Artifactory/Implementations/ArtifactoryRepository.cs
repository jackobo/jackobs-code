using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json.Linq;
using Spark.Infra.Types;

namespace GamesPortal.Service.Artifactory
{
  
    public abstract class ArtifactoryRepository : IArtifactoryRepository
    {
        
        public ArtifactoryRepository(string repositoryName, IArtifactoryRestClientFactory restClientFactory)
        {
            if (string.IsNullOrEmpty(repositoryName))
                throw new ArgumentNullException($"{nameof(repositoryName)} can't be null or empty");
            
            if (restClientFactory == null)
                throw new ArgumentNullException($"{nameof(restClientFactory)} can't be null");

            this.RepositoryName = repositoryName;
            this.RestClientFactory = restClientFactory;
            this.IgnoredRegulations = ReadIgnoredRegulationsFromConfiguration();
        }

        public string RepositoryName { get; private set; }
        private IArtifactoryRestClientFactory RestClientFactory { get; set; }

        protected IArtifactoryRestClient CreateUnauthenticatedRestClient()
        {
            return this.RestClientFactory.CreateUnauthenticatedStorageApi();
        }

        public abstract string GetRootFolderRelativeUrl();
        protected abstract string GetComponentFolderRelativeUrl(int componentId);

        public abstract int[] GetGames();

        private string GetRegulationRelativeUrl(int gameType, string regulation)
        {
            return string.Format("{0}/{1}", GetComponentFolderRelativeUrl(gameType), regulation);
        }

        private string GetVersionFolderRelativeUrl(int gameType, string regulation, string version)
        {
            return string.Format("{0}/{1}", GetRegulationRelativeUrl(gameType, regulation), version);
        }
        
        public string[] IgnoredRegulations { get; set; }
        
        private static string[] ReadIgnoredRegulationsFromConfiguration()
        {
            var ignoredRegulatiosn = System.Configuration.ConfigurationManager.AppSettings["ignoredRegulations"];
            if (string.IsNullOrEmpty(ignoredRegulatiosn))
                return new string[0];

            return ignoredRegulatiosn.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
        }

        public string[] GetComponentRegulations(int gameType)
        {
            try
            {
                var content = CreateUnauthenticatedRestClient().Get(GetComponentFolderRelativeUrl(gameType));
                var storageItem = ParseArtifactoryResponse<ArtifactoryStorageItem>(content);
                return storageItem.children.Where(ch => ch.folder)
                                           .Select(ch => ch.GetUriValue())
                                           .Where(regulatioName => !IgnoredRegulations.Contains(regulatioName, StringComparer.OrdinalIgnoreCase))
                                           .ToArray();
            }
            catch(ArtifactoryException ex)
            {
                if (ex.Errors.Length == 1 && ex.Errors.First().status == WellKnownErrorCodes.NotFound)
                    return new string[0];

                throw;
            }
            
        }

        public string[] GetVersionFolders(int gameType, string regulation)
        {
            try
            {
                var content = CreateUnauthenticatedRestClient().Get(GetRegulationRelativeUrl(gameType, regulation));

                var storageItem = ParseArtifactoryResponse<ArtifactoryStorageItem>(content);

                return storageItem.children.Where(ch => ch.folder).Select(ch => ch.GetUriValue()).ToArray();
            }
            catch(ArtifactoryException ex)
            {
                if (ex.Errors.Length == 1 && ex.Errors.First().status == WellKnownErrorCodes.NotFound)
                    return new string[0];

                throw;
            }
        }


        protected static TResponse ParseArtifactoryResponse<TResponse>(string content, bool throwIfErrors = true) where TResponse : ArtifactoryResponse
        {

            if (throwIfErrors)
                return ArtifactoryResponseExtensions.ParseObject<TResponse>(content);
            else
                return ArtifactoryResponseExtensions.TryParse<TResponse>(content);


        }

        private string TryGetFileName(int gameType, string regulation, string version)
        {
            var restClient = CreateUnauthenticatedRestClient();
            var versionFolderUrl = GetVersionFolderRelativeUrl(gameType, regulation, version);
            var content = restClient.Get(versionFolderUrl);
            var storageItem = ParseArtifactoryResponse<ArtifactoryStorageItem>(content, false);
            if (storageItem.errors.Count > 0 && storageItem.errors.AllNotFound())
                return null;

            var fileName = storageItem.children.Where(ch => !ch.folder).Select(ch => ch.GetUriValue()).FirstOrDefault();

            return fileName;
        }

        public Optional<Artifact> GetArtifact(int gameType, string regulation, string version)
        {
            try
            {
                string fileName = TryGetFileName(gameType, regulation, version);
                if (string.IsNullOrEmpty(fileName))
                    return Optional<Artifact>.None();

                var restClient = CreateUnauthenticatedRestClient();
                var versionFolderUrl = GetVersionFolderRelativeUrl(gameType, regulation, version);

                var artifact = ParseArtifactoryResponse<Artifact>(restClient.Get(string.Format("{0}/{1}", versionFolderUrl, fileName)));
                var propertiesContent = restClient.Get(string.Format("{0}/{1}?properties", versionFolderUrl, fileName));
                
                artifact.Properties = ParseGetPropertiesResponse(propertiesContent);

                return Optional<Artifact>.Some(artifact);
                
                
            }
            catch (Exception ex)
            {
                throw new ApplicationException($"Failed to GetArtifact for GameType = {gameType}; Regulation = {regulation}; Version = {version}; Repository = {this.GetRootFolderRelativeUrl()}", ex);
            }

        }


        private ArtifactoryPropertyCollection ParseGetPropertiesResponse(string content)
        {
            var result = new ArtifactoryPropertyCollection();

            var response = ParseArtifactoryResponse<ArtifactoryResponse>(content, false);

            if (response.errors != null && response.errors.Count > 0)
            {
                if (response.errors.AllNotFound())
                    return result;
                else
                    throw new ArtifactoryException(response.errors);
            }

            var jsonObj = JObject.Parse(content).Properties().First().Value as JObject;

            if (jsonObj == null)
            {
                return result;
            }
            foreach (var property in jsonObj.Properties())
            {
                if (property.Value.Type == JTokenType.Array)
                {
                    var jarray = (JArray)property.Value;
                    result.Add(new ArtifactoryProperty(property.Name, jarray.Values().Select(v => v.ToString()).ToArray()));
                }
                else
                {
                    result.Add(new ArtifactoryProperty(property.Name, property.Value.ToString()));
                }
            }


            return result;
        }

        #region IArtifactoryGamesRepository Members


        public void UpdateArtifactProperties(UpdateArtifactPropertiesRequest request)
        {
            
            var restClient = this.RestClientFactory.CreateAuthenticatedStorageApi();

            var versionFolderUrl = GetVersionFolderRelativeUrl(request.ComponentId, request.Regulation, request.Version);
            var content = restClient.Get(versionFolderUrl);
            var storageItem = ParseArtifactoryResponse<ArtifactoryStorageItem>(content);
            var fileName = storageItem.children.First(ch => !ch.folder).GetUriValue();


            var response = restClient.Put(string.Format("{0}/{1}?properties={2}", 
                                          versionFolderUrl,
                                          fileName,
                                          string.Join("|", request.Properties.Select(p => p.Key + "=" + string.Join(",", p.Values)))));



            if (!string.IsNullOrEmpty(response))
            {
                ParseArtifactoryResponse<ArtifactoryResponse>(response, true);
            }

        }


        public void DeleteArtifactProperties(DeleteArtifactPropertiesRequest request)
        {
            var restClient = this.RestClientFactory.CreateAuthenticatedStorageApi();

            var versionFolderUrl = GetVersionFolderRelativeUrl(request.ComponentId, request.Regulation, request.Version);
            var content = restClient.Get(versionFolderUrl);
            var storageItem = ParseArtifactoryResponse<ArtifactoryStorageItem>(content);
            var fileName = storageItem.children.First(ch => !ch.folder).GetUriValue();


            string response = restClient.Delete(string.Format("{0}/{1}?properties={2}",
                                          versionFolderUrl,
                                          fileName,
                                          string.Join(",", request.Properties)));



            if (!string.IsNullOrEmpty(response))
            {
                ParseArtifactoryResponse<ArtifactoryResponse>(response, true);
            }
        }

       

        #endregion

       
    }

    
}
