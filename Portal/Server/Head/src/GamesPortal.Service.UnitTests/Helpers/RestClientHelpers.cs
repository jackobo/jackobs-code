using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using NSubstitute;

namespace GamesPortal.Service.Helpers
{
    public static class RestClientHelpers
    {
  
        public static IArtifactoryRestClientFactory GetArtifactoryRestClientFactoryStub(params string[] returnedContent)
        {
            var restClient = Substitute.For<IArtifactoryRestClient>();

            if (returnedContent.Length > 0)
            {
                restClient.Get(Arg.Any<string>()).Returns(returnedContent.First(), returnedContent.Skip(1).ToArray());
            }

            var factory = Substitute.For<IArtifactoryRestClientFactory>();
            factory.CreateUnauthenticatedStorageApi().Returns(restClient);

            return factory;
        }


        public static string GetFolderWithSubfoldersResponse(params string[] subFolders)
        {
            return GetFolderResponse(true, subFolders);
        }


        public static string GetFolderWithFileResponse(string fileName)
        {
            return GetFolderResponse(false, fileName);
        }


        public static string GetFolderResponse(bool childsAreFolders, params string[] childItems)
        {
            ArtifactoryStorageItem storageItem = new ArtifactoryStorageItem();
            storageItem.children = childItems.Select(ch => new ArtifactoryStorageItemChild(ch, childsAreFolders)).ToList();
            storageItem.created = new DateTime(2014, 11, 12, 18, 3, 6);
            storageItem.createdBy = "smaster";
            storageItem.errors = new ArtifactoryErrorCollection();
            storageItem.lastModified = new DateTime(2014, 11, 13, 18, 3, 6);
            storageItem.lastUpdated = new DateTime(2014, 11, 13, 18, 3, 6);
            storageItem.modifiedBy = "florin";
            storageItem.path = "/Games";
            storageItem.repo = "repoName";
            storageItem.uri = "http://artifactory.888holdings.corp:8081/artifactory/api/storage/modernGame-local/Games";

            return Newtonsoft.Json.JsonConvert.SerializeObject(storageItem);
        }


        public static string GetFolderResponse(string[] folders, string[] files)
        {
            ArtifactoryStorageItem storageItem = new ArtifactoryStorageItem();
            storageItem.children = folders.Select(ch => new ArtifactoryStorageItemChild(ch, true))
                                   .Union(
                                   files.Select(ch => new ArtifactoryStorageItemChild(ch, false))
                                   )
                                   .ToList();
            storageItem.created = new DateTime(2014, 11, 12, 18, 3, 6);
            storageItem.createdBy = "smaster";
            storageItem.errors = new ArtifactoryErrorCollection();
            storageItem.lastModified = new DateTime(2014, 11, 13, 18, 3, 6);
            storageItem.lastUpdated = new DateTime(2014, 11, 13, 18, 3, 6);
            storageItem.modifiedBy = "florin";
            storageItem.path = "/Games";
            storageItem.repo = "repoName";
            storageItem.uri = "http://artifactory.888holdings.corp:8081/artifactory/api/storage/modernGame-local/Games";

            return Newtonsoft.Json.JsonConvert.SerializeObject(storageItem);
        }

        public static string GetArtifactoryErrorResponse(params string[] errorCodes)
        {
            var response = new ArtifactoryResponse() { errors = new ArtifactoryErrorCollection(errorCodes.Select(er => new ArtifactoryError(er, "Error " + er))) };
            return Newtonsoft.Json.JsonConvert.SerializeObject(response);
        }


        public static string CreateSerializedArtifact(string downloadUrl = "http://artifactorydev.888holdings.corp:8081/artifactory/api/storage/HTML5Game-local/Games/130017/Gibraltar/1.25.0.0/130017-1.25.0.0.zip")
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(CreateArtifact(downloadUrl));
        }

        public static Artifact CreateArtifact(string downloadUrl = "http://artifactorydev.888holdings.corp:8081/artifactory/api/storage/HTML5Game-local/Games/130017/Gibraltar/1.25.0.0/130017-1.25.0.0.zip")
        {
            Artifact storageItem = new Artifact();
            storageItem.created = new DateTime(2014, 11, 12, 18, 3, 6);
            storageItem.createdBy = "smaster";
            storageItem.errors = new ArtifactoryErrorCollection();
            storageItem.lastModified = new DateTime(2014, 11, 13, 18, 3, 6);
            storageItem.lastUpdated = new DateTime(2014, 11, 13, 18, 3, 6);
            storageItem.modifiedBy = "florin";
            storageItem.path = "/Games/130017/Gibraltar/1.25.0.0/130017-1.25.0.0.zip";
            storageItem.repo = "HTML5Game-local";
            storageItem.uri = "http://artifactorydev.888holdings.corp:8081/artifactory/api/storage/HTML5Game-local/Games/130017/Gibraltar/1.25.0.0/130017-1.25.0.0.zip";

            storageItem.checksums = new Artifact.ArtifactChecksums("338f78ca3bcd89428b306e4a9a5d8768f6115248", "4facf0fe13016258d5efdadd8892e7e2");
            storageItem.originalChecksums = new Artifact.ArtifactChecksums("338f78ca3bcd89428b306e4a9a5d8768f6115248", "4facf0fe13016258d5efdadd8892e7e2");

            storageItem.downloadUri = downloadUrl;
            return storageItem;
        }

        public static string GetArtifactoryErrorResponse(params int[] errorCodes)
        {
            return GetArtifactoryErrorResponse(errorCodes.Select(er => er.ToString()).ToArray());
        }


        public static string GetPropertiesResponse(params KeyValuePair<string, string>[] propertyKeyValue)
        {
            return string.Format("{{\"properties\" : {{{0}}}}}", string.Join(",", propertyKeyValue.Select(p => string.Format("\"{0}\" : [\"{1}\"]", p.Key, p.Value))));
        }
    }
}
