using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Spark.Infra.Configurations;
using RestSharp.Authenticators;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryRestClientFactory : IArtifactoryRestClientFactory
    {
        public ArtifactoryRestClientFactory(IConfigurationReader configurationReader)
        {
            this.ConfigurationReader = configurationReader;   
        }

        

        IConfigurationReader ConfigurationReader { get; set; }
        

        #region IArtifactoryRestClientFactory Members

        public IArtifactoryRestClient CreateUnauthenticatedStorageApi()
        {
            return new ArtifactoryRestClient(ConfigurationReader.ReadSection<ArtifactorySettings>().BaseUrl, "artifactory/api/storage");
        }

        public IArtifactoryRestClient CreateAuthenticatedStorageApi()
        {
            var settings = ConfigurationReader.ReadSection<ArtifactorySettings>();
            return new ArtifactoryRestClient(settings.BaseUrl,
                                             settings.UserName, 
                                             settings.Password,
                                             "artifactory/api/storage");
        }



        #endregion




        #region IArtifactoryRestClientFactory Members


        public IArtifactoryRestClient CreateAuthenticatedUiApi()
        {
            var settings = ConfigurationReader.ReadSection<ArtifactorySettings>();
            return new ArtifactoryRestClient(settings.BaseUrl,
                                             settings.UserName,
                                             settings.Password,
                                             "artifactory/ui");
        }

        #endregion
    }
}
