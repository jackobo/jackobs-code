using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.Helpers;
using NSubstitute;
using NUnit.Framework;

namespace GamesPortal.Service
{
    [TestFixture]
    public class ArtifactoryGamesRepositoryTests
    {

        private GamesRepository CreateArtifactoryGamesRepository(string repoName, IArtifactoryRestClientFactory restClientFactory)
        {
            return new GamesRepository(repoName, "Games", restClientFactory);
        }

        [Test]
        public void GetGames_IsCallingArtifactoryWithTheRightUrl()
        {
            string actualRelativeUrl = null;

            var restClient = Substitute.For<IArtifactoryRestClient>();
            restClient.Get(Arg.Do<string>(url => actualRelativeUrl= url)).Returns(GetDefaultGamesFolderContent());

            var factory = Substitute.For<IArtifactoryRestClientFactory>();
            factory.CreateUnauthenticatedStorageApi().Returns(restClient);

            var repository = CreateArtifactoryGamesRepository("RepoName", factory);
            repository.GetGames();

            Assert.AreEqual("RepoName/Games", actualRelativeUrl);
        }

        [Test]
        public void GetGames_ReturnsAndArrayOfIntegersRepresentingTheGameTypes()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetFolderWithSubfoldersResponse("13001", "13002")));
            var gameTypes = repository.GetGames();
            Assert.AreEqual(2, gameTypes.Length);
            Assert.AreEqual(13001, gameTypes[0]);
            Assert.AreEqual(13002, gameTypes[1]);
        }

        [Test]
        public void GetGames_WhenArtifactoryReturnsError_ThrowException()
        {
            
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetArtifactoryErrorResponse(401)));
            Assert.Throws<ArtifactoryException>(() => repository.GetGames());
        }
        
        [Test]
        public void GetGames_ShouldIgnoreFoldersThatCannotBeConvertedToAGameType()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetFolderWithSubfoldersResponse("13001", "13002", "Archive")));
            var gameTypes = repository.GetGames();
            Assert.AreEqual(2, gameTypes.Length);
            Assert.AreEqual(13001, gameTypes[0]);
            Assert.AreEqual(13002, gameTypes[1]);
        }

        [Test]
        public void GetGameRegulations_IsCallingArtifactoryWithTheRightUrl()
        {
            string actualRelativeUrl = null;

            var restClient = Substitute.For<IArtifactoryRestClient>();
            restClient.Get(Arg.Do<string>(url => actualRelativeUrl = url)).Returns(GetDefaultGamesFolderContent());

            var factory = Substitute.For<IArtifactoryRestClientFactory>();
            factory.CreateUnauthenticatedStorageApi().Returns(restClient);

            var repository = CreateArtifactoryGamesRepository("RepoName", factory);
            repository.GetComponentRegulations(130017);

            Assert.AreEqual("RepoName/Games/130017", actualRelativeUrl);
        }



        [Test]
        public void GetGameRegulations_ShouldOnlyLookForFolders()
        {
            string actualRelativeUrl = null;

            var restClient = Substitute.For<IArtifactoryRestClient>();
            restClient.Get(Arg.Do<string>(url => actualRelativeUrl = url)).Returns(RestClientHelpers.GetFolderResponse(new string[] { "Gibraltar" }, new string[] { "test.jpg" }));

            var factory = Substitute.For<IArtifactoryRestClientFactory>();
            factory.CreateUnauthenticatedStorageApi().Returns(restClient);

            var repository = CreateArtifactoryGamesRepository("RepoName", factory);
            var regulations = repository.GetComponentRegulations(130017);

            Assert.AreEqual(1, regulations.Length);
        }

        [Test]
        public void GetGameRegulations_ShouldNotReturnIgnoredRegulations()
        {
            string actualRelativeUrl = null;

            var restClient = Substitute.For<IArtifactoryRestClient>();
            restClient.Get(Arg.Do<string>(url => actualRelativeUrl = url)).Returns(RestClientHelpers.GetFolderResponse(new string[] { "Gibraltar", "Delaware", "Spain" }, new string[] { "test.jpg" }));

            var factory = Substitute.For<IArtifactoryRestClientFactory>();
            factory.CreateUnauthenticatedStorageApi().Returns(restClient);

            var repository = CreateArtifactoryGamesRepository("RepoName", factory);
            repository.IgnoredRegulations = new string[] { "delaware" };
            var regulations = repository.GetComponentRegulations(130017);

            Assert.AreEqual(2, regulations.Length);
            Assert.IsFalse(regulations.Any(r => r == "Delaware"));
        }


        [Test]
        public void GetGameRegulations_ReturnsAnArrayContainingRegulationsNames()
        {
            
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetFolderWithSubfoldersResponse("888Italy", "Gibraltar", "Spain")));
            var regulations = repository.GetComponentRegulations(130017);

            Assert.AreEqual(3, regulations.Length);
            Assert.AreEqual("888Italy", regulations[0]);
            Assert.AreEqual("Gibraltar", regulations[1]);
            Assert.AreEqual("Spain", regulations[2]);

        }


        [Test]
        public void GetGameRegulations_WhenArtifactoryReturnsError_ThrowException()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetArtifactoryErrorResponse(123, 456)));
            Assert.Throws<ArtifactoryException>(() => repository.GetComponentRegulations(130017));

        }


        [Test]
        public void GetVersionFolders_IsCallingArtifactoryWithTheRightUrl()
        {

            string actualRelativeUrl = null;

            var restClient = Substitute.For<IArtifactoryRestClient>();
            restClient.Get(Arg.Do<string>(url => actualRelativeUrl = url)).Returns(GetDefaultGamesFolderContent());

            var factory = Substitute.For<IArtifactoryRestClientFactory>();
            factory.CreateUnauthenticatedStorageApi().Returns(restClient);


            var repository = CreateArtifactoryGamesRepository("RepoName", factory);
            repository.GetVersionFolders(130017, "Gibraltar");

            Assert.AreEqual("RepoName/Games/130017/Gibraltar", actualRelativeUrl);
        }

        [Test]
        public void GetVersionFolders_WhenArtifactoryReturnsErrorOtherThan404_ThrowException()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetArtifactoryErrorResponse(401, 402)));
            Assert.Throws<ArtifactoryException>(() => repository.GetVersionFolders(130017, "Gibraltar"));
        }

        [Test]
        public void GetVersionFolders_WhenArtifactoryReturnsError404_ReturnsEmptyArray()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetArtifactoryErrorResponse(404)));
            var versions = repository.GetVersionFolders(130017, "Gibraltar");
            Assert.AreEqual(0, versions.Length);
        }

        [Test]
        public void GetVersionFolders_WhenArtifactoryReturnsError404AndSomeOtherError_ThrowException()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetArtifactoryErrorResponse(404, 401)));
            Assert.Throws<ArtifactoryException>(() => repository.GetVersionFolders(130017, "Gibraltar"));
        }

        [Test]
        public void GetVersionFolders_ReturnsAnArrayContainingVersionFolderNames()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetFolderWithSubfoldersResponse("1.0.0.0", "1.0.0.1")));
            var versions = repository.GetVersionFolders(130017, "Gibraltar");

            Assert.AreEqual(2, versions.Length);
            Assert.AreEqual("1.0.0.0", versions[0]);
            Assert.AreEqual("1.0.0.1", versions[1]);
        }


        [Test]
        public void GetArtifact_Makes3CallsToArtifactory()
        {
            var calledUrls = new List<string>();

            var restClient = Substitute.For<IArtifactoryRestClient>();
            restClient.Get(Arg.Do<string>(url => calledUrls.Add(url))).Returns(RestClientHelpers.GetFolderWithFileResponse("130004-2.0.8.zip"),
                                                        RestClientHelpers.GetFolderWithSubfoldersResponse(),
                                                        RestClientHelpers.GetPropertiesResponse(new KeyValuePair<string, string>("p1", "v1"),
                                                                                                new KeyValuePair<string, string>("p2", "v2")));
            

            var factory = Substitute.For<IArtifactoryRestClientFactory>();
            factory.CreateUnauthenticatedStorageApi().Returns(restClient);

            var repository = CreateArtifactoryGamesRepository("RepoName", factory);

            repository.GetArtifact(130004, "Gibraltar", "2.0.8");

            Assert.AreEqual(3, calledUrls.Count);
            Assert.AreEqual("RepoName/Games/130004/Gibraltar/2.0.8", calledUrls[0]);
            Assert.AreEqual("RepoName/Games/130004/Gibraltar/2.0.8/130004-2.0.8.zip", calledUrls[1]);
            Assert.AreEqual("RepoName/Games/130004/Gibraltar/2.0.8/130004-2.0.8.zip?properties", calledUrls[2]);
        }

        [Test]
        public void GetVersionProperties_WhenNoPropertiesCouldBeFoundErrorIsReturned_ReturnsEmptyArray()
        {
            
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(
                                                                                                RestClientHelpers.GetFolderWithFileResponse("130004-2.0.8.zip"),
                                                                                                RestClientHelpers.GetFolderWithSubfoldersResponse(),
                                                                                                RestClientHelpers.GetArtifactoryErrorResponse(WellKnownErrorCodes.NotFound)));

            repository.GetArtifact(130004, "Gibraltar", "2.0.8")
                      .Do(artifact => Assert.AreEqual(0, artifact.Properties.Count));

        }




        [Test]
        public void GetVersionProperties_WhenAnErrorCodeOtherThanNotFoundIsReturned_ThrowsException()
        {

            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(RestClientHelpers.GetFolderWithFileResponse("130004-2.0.8.zip"),
                                                                                                RestClientHelpers.GetArtifactoryErrorResponse(501)));

            Assert.Throws<ApplicationException>(() => repository.GetArtifact(130004, "Gibraltar", "2.0.8"));

        }


        [Test]
        public void GetVersionProperties_ReturnsAnArrayOfProperties()
        {
            var repository = CreateArtifactoryGamesRepository("RepoName",
                                                            RestClientHelpers.GetArtifactoryRestClientFactoryStub(
                                                                                                RestClientHelpers.GetFolderWithFileResponse("130004-2.0.8.zip"),
                                                                                                RestClientHelpers.GetFolderWithSubfoldersResponse(),
                                                                                                RestClientHelpers.GetPropertiesResponse(new KeyValuePair<string, string>("p1", "v1"), 
                                                                                                                      new KeyValuePair<string, string>("p2", "v2")
                                                                                                                             )));
            Artifact storageItem = null;
            repository.GetArtifact(130004, "Gibraltar", "2.0.8")
                      .Do(artifact => storageItem = artifact);
            var properties = storageItem.Properties;
            Assert.AreEqual(2, properties.Count);
            Assert.AreEqual("p1", properties[0].Key);
            Assert.AreEqual("v1", properties[0].ConcatValues());
            Assert.AreEqual("p2", properties[1].Key);
            Assert.AreEqual("v2", properties[1].ConcatValues());

        }


        [Test]
        public void UpdateArtifactProperties_ShouldCreateAuthenticatedRestClient()
        {

            var artifactoryRestClient = Substitute.For<IArtifactoryRestClient>();
            artifactoryRestClient.Get(Arg.Any<string>()).Returns(RestClientHelpers.GetFolderWithFileResponse("130017-1.2.3.zip"));

            var artifactoryRestClientFactory = Substitute.For<IArtifactoryRestClientFactory>();
            artifactoryRestClientFactory.CreateAuthenticatedStorageApi().Returns(artifactoryRestClient);
            
            var repository = CreateArtifactoryGamesRepository("RepoName", artifactoryRestClientFactory);
            repository.UpdateArtifactProperties(new UpdateArtifactPropertiesRequest(130017, "Gibraltar", "1.2.3", new ArtifactoryProperty("NDL.State", "Approved")));

            artifactoryRestClientFactory.Received().CreateAuthenticatedStorageApi();
            

        }

        [Test]
        public void UpdateArtifactProperties_ShouldCallPutMethodInTheArtifactoryRestClient()
        {
            var artifactoryRestClient = Substitute.For<IArtifactoryRestClient>();
            artifactoryRestClient.Get(Arg.Any<string>()).Returns(RestClientHelpers.GetFolderWithFileResponse("130017-1.2.3.zip"));
            

            var artifactoryRestClientFactory = Substitute.For<IArtifactoryRestClientFactory>();
            artifactoryRestClientFactory.CreateAuthenticatedStorageApi().Returns(artifactoryRestClient);

            var repository = CreateArtifactoryGamesRepository("RepoName", artifactoryRestClientFactory);
            repository.UpdateArtifactProperties(new UpdateArtifactPropertiesRequest(130017, "Gibraltar", "1.2.3", new ArtifactoryProperty("NDL.State", "Approved")));

            artifactoryRestClient.Received().Put(Arg.Any<string>());

        }

        [Test]
        public void UpdateArtifactProperties_ProvidesTheCorrectRelativUrlToThePutMethod()
        {

            var artifactoryRestClient = Substitute.For<IArtifactoryRestClient>();
            artifactoryRestClient.Get(Arg.Any<string>()).Returns(RestClientHelpers.GetFolderWithFileResponse("130017-1.2.3.zip"));
            
            string actualUrl = null;
            artifactoryRestClient.Put(Arg.Do<string>(url => actualUrl = url)).Returns("");
            
            var artifactoryRestClientFactory = Substitute.For<IArtifactoryRestClientFactory>();
            artifactoryRestClientFactory.CreateAuthenticatedStorageApi().Returns(artifactoryRestClient);
            artifactoryRestClientFactory.CreateUnauthenticatedStorageApi().Returns(artifactoryRestClient);

            

            var repository = CreateArtifactoryGamesRepository("RepoName", artifactoryRestClientFactory);
            repository.UpdateArtifactProperties(new UpdateArtifactPropertiesRequest(130017, "Gibraltar", "1.2.3", 
                                                                                    new ArtifactoryProperty("NDL.State", "Approved"), 
                                                                                    new ArtifactoryProperty("ClientVersion", "v1", "v2")));

            Assert.AreEqual("RepoName/Games/130017/Gibraltar/1.2.3/130017-1.2.3.zip?properties=NDL.State=Approved|ClientVersion=v1,v2",
                            actualUrl);

        }

        [Test]
        public void UpdateArtifactProperties_IfPutReturnsError_ShouldThrowArtifactoryException()
        {

            var artifactoryRestClient = Substitute.For<IArtifactoryRestClient>();
            artifactoryRestClient.Get(Arg.Any<string>()).Returns(RestClientHelpers.GetFolderWithFileResponse("130017-1.2.3.zip"));
            artifactoryRestClient.Put(Arg.Any<string>()).Returns(RestClientHelpers.GetArtifactoryErrorResponse(401));

            var artifactoryRestClientFactory = Substitute.For<IArtifactoryRestClientFactory>();
            artifactoryRestClientFactory.CreateAuthenticatedStorageApi().Returns(artifactoryRestClient);
            artifactoryRestClientFactory.CreateUnauthenticatedStorageApi().Returns(artifactoryRestClient);


            var repository = CreateArtifactoryGamesRepository("RepoName", artifactoryRestClientFactory);

            Assert.Throws<ArtifactoryException>(() => repository.UpdateArtifactProperties(new UpdateArtifactPropertiesRequest(130017, "Gibraltar", "1.2.3", new ArtifactoryProperty("NDL.State", "Approved"))));

        }



        [TestCase(null)]
        [TestCase("")]
        public void UpdateArtifactProperties_IfPutReturnsEmptyStringOrNull_ShouldNOTThrowException(string putReturn)
        {

            var artifactoryRestClient = Substitute.For<IArtifactoryRestClient>();
            artifactoryRestClient.Get(Arg.Any<string>()).Returns(RestClientHelpers.GetFolderWithFileResponse("130017-1.2.3.zip"));
            artifactoryRestClient.Put(Arg.Any<string>()).Returns(putReturn);

            var artifactoryRestClientFactory = Substitute.For<IArtifactoryRestClientFactory>();
            artifactoryRestClientFactory.CreateAuthenticatedStorageApi().Returns(artifactoryRestClient);
            artifactoryRestClientFactory.CreateUnauthenticatedStorageApi().Returns(artifactoryRestClient);


            var repository = CreateArtifactoryGamesRepository("RepoName", artifactoryRestClientFactory);

            repository.UpdateArtifactProperties(new UpdateArtifactPropertiesRequest(130017, "Gibraltar", "1.2.3", new ArtifactoryProperty("NDL.State", "Approved")));



        }

        private string GetDefaultGamesFolderContent()
        {
            return RestClientHelpers.GetFolderWithSubfoldersResponse("130017", "130043");
        }



        private string GetDefaultGameTypeFolderContent()
        {
            return RestClientHelpers.GetFolderWithSubfoldersResponse("888Italy", "Gibraltar");
        }


        private string GetDefaultRegulationFolderContent()
        {
            return RestClientHelpers.GetFolderWithSubfoldersResponse("1.0.0.0", "1.0.0.1");
        }

      

    }
}
