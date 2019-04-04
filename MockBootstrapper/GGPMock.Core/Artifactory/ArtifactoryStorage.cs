using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GGPGameServer.ApprovalSystem.Common;
using Newtonsoft.Json.Linq;

namespace GGPMockBootstrapper.Artifactory
{
    public class ArtifactoryStorage
    {
        RestSharp.RestClient _restClient;
        
        public ArtifactoryStorage(string mainFolder)
        {
            this.MainFolder = mainFolder;
            _restClient = new RestSharp.RestClient(System.Configuration.ConfigurationManager.AppSettings["ArtifactoryUri"]);
        }


        string MainFolder { get; set; }

        
        string BuildStorageGetRequest(string relativeUri)
        {
            if(!relativeUri.StartsWith("/"))
                relativeUri = "/" + relativeUri;

            return string.Format("artifactory/api/storage{0}", relativeUri);
        }

       

        public string[] GetRegulations(int gameType)
        {
            var storageItem = GetStorageItemFromRelativeUri(BuildStorageGetRequest(string.Format("{0}/Games/{1}", this.MainFolder, gameType)));

            return storageItem.children.Select(ch => ch.GetUriValue()).ToArray();
        }


        public VersionNumber[] GetGameVersions(int gameType, string regulation)
        {
            var storageItem = GetStorageItemFromRelativeUri(BuildStorageGetRequest(string.Format("{0}/Games/{1}/{2}", this.MainFolder, gameType, regulation)));

            return storageItem.children.Select(ch => new VersionNumber(ch.GetUriValue())).ToArray();
        }


        public VersionNumber[] GetChillWrapperVersions(string regulation)
        {

            var regulationFolder = GetWrapperRegulationFolder(regulation);
            if (regulationFolder == null)
                return new VersionNumber[0];

            return regulationFolder.children.Select(ch => {
#warning This is a temporary hot fix until we clarify what is the deal with the chil version containing the dev sufix
                try
                {
                    return new VersionNumber(ch.GetUriValue());
                }
                catch
                {
                    return null;
                }
            })
            .Where(v => v != null)
            .ToArray();

        }


        private StorageItem GetWrapperRegulationFolder(string regulation)
        {
            var mainStorageItem = GetStorageItemFromRelativeUri(BuildStorageGetRequest(this.MainFolder));
            var wrapperChildFolder = mainStorageItem.children.FirstOrDefault(ch => ch.GetUriValue().StartsWith("Wrapper"));


            var url = BuildStorageGetRequest(this.MainFolder + wrapperChildFolder.uri);
            var wrapperFolder = GetStorageItemFromRelativeUri(url);

            var chillChildFolder = wrapperFolder.children.FirstOrDefault(ch => ch.GetUriValue().StartsWith("chill"));

            if (chillChildFolder != null)
            {
                url += chillChildFolder.uri;
                wrapperFolder = GetStorageItemFromRelativeUri(url);
            }

            var wrapperRegulationChildFolder = wrapperFolder.children.FirstOrDefault(ch => 0 == string.Compare(ch.GetUriValue(), regulation, true));

            if (wrapperRegulationChildFolder == null)
                return null;


            return GetStorageItemFromRelativeUri(url + wrapperRegulationChildFolder.uri);
        }

        public string GetWrapperChillDownloadUrl(string regulation, VersionNumber version)
        {
            var regulationFolder = GetWrapperRegulationFolder(regulation);


            var versionUri = regulationFolder.uri + "/" + version.ParsedVersion;

            var file = GetStorageItemFromAbsoluteUri(versionUri).children.FirstOrDefault(ch => !ch.folder);

            return GetStorageItemFromAbsoluteUri(versionUri + file.uri).downloadUri;



        }


        public string GetGameDownloadUrl(int gameType, string regulation, VersionNumber version)
        {
            var versionRelativeUrl = BuildStorageGetRequest(string.Format("{0}/Games/{1}/{2}/{3}", this.MainFolder, gameType, regulation, version.ParsedVersion));

            var versionFolder = GetStorageItemFromRelativeUri(versionRelativeUrl);

            var file = versionFolder.children.FirstOrDefault(ch => !ch.folder);

            var fileStorageItem = GetStorageItemFromRelativeUri(versionRelativeUrl + file.uri);
            return fileStorageItem.downloadUri;

        }



        public StorageItemProperty[] GetItemProperties(string url)
        {
            UriBuilder builder = new UriBuilder(url);

            var response = _restClient.Execute(new RestSharp.RestRequest(builder.Path + "?properties", RestSharp.Method.GET));


            var result = new List<StorageItemProperty>();

            var jsonObj = JObject.Parse(response.Content).Properties().First().Value as JObject;

            if (jsonObj == null)
            {
                Console.WriteLine("No properties found for " + url);
                return result.ToArray();
            }
            foreach (var property in jsonObj.Properties())
            {
                if (property.Value.Type == JTokenType.Array)
                {
                    var jarray = (JArray)property.Value;
                    result.Add(new StorageItemProperty(property.Name, jarray.Values().Select(v => v.ToString()).ToArray()));
                }
                else
                {
                    result.Add(new StorageItemProperty(property.Name, property.Value.ToString()));
                }
            }


            return result.ToArray();

        }


        

        private StorageItem GetStorageItemFromRelativeUri(string relativeUri)
        {
            var request = new RestSharp.RestRequest(relativeUri, RestSharp.Method.GET);
            var response = _restClient.Execute<StorageItem>(request);
            if (response.ErrorException != null)
            {

                throw new ArtifactoryException(string.Format("Failed to get storage item from Artifactory. Request URL: {0}/{1}", _restClient.BaseUrl, relativeUri), response.ErrorException);
            }

            return response.Data;
        }


        private StorageItem GetStorageItemFromAbsoluteUri(string uri)
        {
            UriBuilder uriBuilder = new UriBuilder(uri);
            var request = new RestSharp.RestRequest(uriBuilder.Path, RestSharp.Method.GET);
            var response = _restClient.Execute<StorageItem>(request);
            if (response.ErrorException != null)
            {
                throw new ArtifactoryException(string.Format("Failed to get storage item from Artifactory. Request URL: {0}/{1}", _restClient.BaseUrl, uriBuilder.Path), response.ErrorException);
            }

            return response.Data;
        }


        
    }
}
