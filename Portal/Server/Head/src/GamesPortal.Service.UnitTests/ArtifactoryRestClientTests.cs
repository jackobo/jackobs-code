using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using NSubstitute;
using NUnit.Framework;
using RestSharp;

namespace GamesPortal.Service
{
    
    public abstract class ArtifactoryRestClientTests
    {

        protected abstract string ExecuteMethod(ArtifactoryRestClient restClient, string url);
        protected abstract Method GetHttpMethod();

        [TestCase("http://artifactory.888holdings.corp:8081/")]
        [TestCase("http://artifactory.888holdings.corp:8081")]
        public void Constructor_Unauthenticated_SetsTheCorrectRestClientBaseUrl(string artifactoryBaseUrl)
        {
            var restClient = Substitute.For<IRestClient>();
            var artifactoryRestClient = new ArtifactoryRestClient(artifactoryBaseUrl, restClient, "artifactory/api/storage");

            Assert.AreEqual(new Uri("http://artifactory.888holdings.corp:8081/artifactory/api/storage"), restClient.BaseUrl);
        }

        [TestCase("http://artifactory.888holdings.corp:8081/")]
        [TestCase("http://artifactory.888holdings.corp:8081")]
        public void Constructor_Authenticated_SetsTheCorrectRestClientBaseUrl(string artifactoryBaseUrl)
        {
            var restClient = Substitute.For<IRestClient>();
            var artifactoryRestClient = new ArtifactoryRestClient(artifactoryBaseUrl, "user", "pass", restClient, "artifactory/api/storage");

            Assert.AreEqual(new Uri("http://artifactory.888holdings.corp:8081/artifactory/api/storage"), restClient.BaseUrl);
        }

        [Test]
        public void Constructor_Authenticated_SetsTheRestClientAuthenticator()
        {
            var restClient = Substitute.For<IRestClient>();
            var artifactoryRestClient = new ArtifactoryRestClient("http://artifactory.888holdings.corp:8081", "user", "pass", restClient, "artifactory/api/storage");


            Assert.IsNotNull(restClient.Authenticator);
        }


        
        [Test]
        public void WhenCallingExecute_ProvideTheRightResourceUri()
        {
            
            var restClient = Substitute.For<IRestClient>();
            
            IRestRequest actualRequest = null;
            restClient.Execute(Arg.Do<IRestRequest>(r => actualRequest = r)).Returns(CreateRestResponse());
            

            var artifactoryRestClient = CreateUnauthenticatedArtifactoryRestClient(restClient);
            ExecuteMethod(artifactoryRestClient,"modernGame-local/Games");

            Assert.AreEqual("modernGame-local/Games", actualRequest.Resource);
        }

        [Test]
        public void WhenCallingExecute_ProvideTheRightRESTMethod()
        {

            var restClient = Substitute.For<IRestClient>();

            IRestRequest actualRequest = null;
            restClient.Execute(Arg.Do<IRestRequest>(r => actualRequest = r)).Returns(CreateRestResponse());
            

            var artifactoryRestClient = CreateUnauthenticatedArtifactoryRestClient(restClient);
            ExecuteMethod(artifactoryRestClient, "modernGame-local/Games");

            Assert.AreEqual(GetHttpMethod(), actualRequest.Method);
        }

        
        [Test]
        public void WhenCallingExecute_ReturnsTheResponseContent()
        {

            var restClient = Substitute.For<IRestClient>();

            var request = CreateRestResponse("test");
            restClient.Execute((IRestRequest)null).ReturnsForAnyArgs(request);
            

            var artifactoryRestClient = CreateUnauthenticatedArtifactoryRestClient(restClient);
            var response = ExecuteMethod(artifactoryRestClient, "modernGame-local/Games");

            Assert.AreEqual("test", response);
        }

        [Test]
        public void WhenCallingExecuteReturnsException_ThrowArtifactoryException()
        {

            var restClient = Substitute.For<IRestClient>();

            var request = CreateRestResponse("test", new ApplicationException());
            restClient.Execute(Arg.Any<IRestRequest>()).Returns(request);
            
            var artifactoryRestClient = CreateUnauthenticatedArtifactoryRestClient(restClient);

            Assert.Throws<ArtifactoryException>(() => ExecuteMethod(artifactoryRestClient, "modernGame-local/Games"));
        }

        [Test]
        public void WhenCallingExecuteReturnsException_ThrownArtifactoryExceptionMessageShouldContainTheUrl()
        {

            var restClient = Substitute.For<IRestClient>();

            var request = CreateRestResponse("test", new ApplicationException());
            restClient.Execute(Arg.Any<IRestRequest>()).Returns(request);
            
            var artifactoryRestClient = CreateUnauthenticatedArtifactoryRestClient(restClient, "http://artifactory.888holdings.corp:8081");
            try
            {
                ExecuteMethod(artifactoryRestClient, "modernGame-local/Games");
                Assert.Fail();
            }
            catch (ArtifactoryException artEx)
            {
                Assert.IsTrue(artEx.Message.Contains("http://artifactory.888holdings.corp:8081/artifactory/api/storage/modernGame-local/Games"));
            }
        }

        [Test]
        public void WhenCallingExecuteReturnsException_ThrownArtifactoryExceptionShouldContainInnerException()
        {

            var restClient = Substitute.For<IRestClient>();

            var expectedException = new ApplicationException();

            var request = CreateRestResponse("test", expectedException);
            restClient.Execute(Arg.Any<IRestRequest>()).Returns(request);
            
            var artifactoryRestClient = CreateUnauthenticatedArtifactoryRestClient(restClient, "http://artifactory.888holdings.corp:8081");
            try
            {
                ExecuteMethod(artifactoryRestClient, "modernGame-local/Games");
                Assert.Fail();
            }
            catch (ArtifactoryException artEx)
            {
                Assert.IsTrue(object.ReferenceEquals(expectedException, artEx.InnerException));
            }
        }

        [Test]
        public void WhenCallingExecute_DonThrowArtifactoryException_IfAfterTheFirstRetryTheRequestIsSuccessfull()
        {

            var restClient = Substitute.For<IRestClient>();
            var r1 = CreateRestResponse("test", new ApplicationException());
            var r2 = CreateRestResponse("test");
            restClient.Execute(Arg.Any<IRestRequest>()).Returns(r1, r2);

            //restClient.Expect(rc => rc.Execute(null)).IgnoreArguments().Return(CreateRestResponse("test", new ApplicationException())).Repeat.Once();
            //restClient.Expect(rc => rc.Execute(null)).IgnoreArguments().Return(CreateRestResponse("test")).Repeat.Once();

            var artifactoryRestClient = CreateUnauthenticatedArtifactoryRestClient(restClient, "http://artifactory.888holdings.corp:8081");
            artifactoryRestClient.MaxRetries = 3;

            ExecuteMethod(artifactoryRestClient, "modernGame-local/Games");

            restClient.Received(2).Execute(Arg.Any<IRestRequest>());
        }


       
        protected ArtifactoryRestClient CreateUnauthenticatedArtifactoryRestClient(IRestClient restClient, string baseUrl = "http://artifactory.888holdings.corp:8081" )
        {
            return new ArtifactoryRestClient(baseUrl, restClient, "artifactory/api/storage") { SleepTimeBetweenRetries = TimeSpan.FromMilliseconds(1) };
        }

        protected ArtifactoryRestClient CreateAuthenticatedArtifactoryRestClient(IRestClient restClient, string baseUrl = "http://artifactory.888holdings.corp:8081", string userName = "user", string password = "pass")
        {
            return new ArtifactoryRestClient(baseUrl, userName, password, restClient, "artifactory/api/storage") { SleepTimeBetweenRetries = TimeSpan.FromMilliseconds(1) };
        }


        protected IRestResponse CreateRestResponse(string content = null, Exception ex = null)
        {
            var response = Substitute.For<IRestResponse>();

            if (content != null)
            {
                response.Content.Returns(content);
            }
            if (ex != null)
            {
                response.ErrorException.Returns(ex);
            }

            return response;
        }

    }

    [TestFixture]
    public class ArtifactoryRestClient_Get_Tests : ArtifactoryRestClientTests
    {

        protected override string ExecuteMethod(ArtifactoryRestClient restClient, string url)
        {
            return restClient.Get(url);
        }

        protected override Method GetHttpMethod()
        {
            return Method.GET;
        }
    }

    
    [TestFixture]
    public class ArtifactoryRestClient_Put_Tests : ArtifactoryRestClientTests
    {
      

        protected override string ExecuteMethod(ArtifactoryRestClient restClient, string url)
        {
            return restClient.Put(url);
        }

        protected override Method GetHttpMethod()
        {
            return Method.PUT;
        }
    }
    
}
