using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using RestSharp.Authenticators;

namespace GamesPortal.Service.Artifactory
{
    public class ArtifactoryRestClient : IArtifactoryRestClient
    {
        RestSharp.IRestClient _restClient;
        
        public ArtifactoryRestClient(string artifactoryBaseUrl, string urlFragment)
            : this(artifactoryBaseUrl, new RestSharp.RestClient(), urlFragment)
        {
        }

        public ArtifactoryRestClient(string artifactoryBaseUrl, RestSharp.IRestClient restClient, string urlFragment)
        {
            ConfigureRestClient(artifactoryBaseUrl, restClient, urlFragment);
            _restClient = restClient;
        }

        public ArtifactoryRestClient(string artifactoryBaseUrl, string userName, string password, RestSharp.IRestClient restClient, string urlFragment)
        {
            ConfigureRestClient(artifactoryBaseUrl, restClient, urlFragment);
            _restClient = restClient;
            _restClient.Authenticator = new HttpBasicAuthenticator(userName, password);
        }

        public ArtifactoryRestClient(string artifactoryBaseUrl, string userName, string password, string urlFragment)
            : this(artifactoryBaseUrl, userName, password, new RestSharp.RestClient(), urlFragment)
        {
          
        }


        int _maxRetries = 3;
        public int MaxRetries
        {
            get { return _maxRetries; }
            set { _maxRetries = value; }
        }

        private TimeSpan _sleepTimeBetweenRetries = TimeSpan.FromMilliseconds(250);

        public TimeSpan SleepTimeBetweenRetries
        {
            get { return _sleepTimeBetweenRetries; }
            set { _sleepTimeBetweenRetries = value; }
        }

        private void ConfigureRestClient(string artifactoryBaseUrl, RestSharp.IRestClient restClient, string urlFragment)
        {
            if (!artifactoryBaseUrl.EndsWith("/"))
                artifactoryBaseUrl += "/";

            restClient.BaseUrl = new Uri(artifactoryBaseUrl + urlFragment);
        }

        public string Get(string relativeUri)
        {
            return ExecuteMethodWithRetry(relativeUri, RestSharp.Method.GET);
        }

        private string ExecuteMethodWithRetry(string relativeUri, RestSharp.Method httpMethod)
        {
            RestSharp.IRestResponse response = null;

            int currentRetry = 0;
            do
            {
                currentRetry++;
                response = _restClient.Execute(new RestSharp.RestRequest(relativeUri, httpMethod));

                if (response.ErrorException != null)
                    Thread.Sleep(SleepTimeBetweenRetries);


            } while (response.ErrorException != null && currentRetry < this.MaxRetries);


            if (response.ErrorException != null)
            {
                throw new ArtifactoryException(string.Format("Failed to execute {0} . Request URL: {1}/{2}", httpMethod, _restClient.BaseUrl, relativeUri), response.ErrorException);
            }

            return response.Content;
        }

        #region IArtifactoryRestClient Members
        
        public string Put(string relativeUri)
        {
            return ExecuteMethodWithRetry(relativeUri, RestSharp.Method.PUT);
        }

        #endregion

        #region IArtifactoryRestClient Members


        public string Delete(string relativeUri)
        {
            return ExecuteMethodWithRetry(relativeUri, RestSharp.Method.DELETE);
        }

        #endregion
    }
}
