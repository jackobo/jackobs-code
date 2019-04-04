using System;
using System.Collections.Generic;
using System.Linq;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.Helpers;
using GGPGameServer.ApprovalSystem.Common;
using NUnit.Framework;
using NSubstitute;
using static GamesPortal.Service.Helpers.MockRecordsFactory;
using static GamesPortal.Service.Helpers.Utils;
using static GamesPortal.Service.Helpers.ArtifactoryGatewayHelper;
using GGPGameServer.ApprovalSystem.Common.Databases;

namespace GamesPortal.Service
{
    [TestFixture]
    public partial class ArtifactoryGameSynchronizerTests
    {
        IGamesPortalInternalServices _internalServices;
        ArtifactoryGameSynchronizer _synchronizer;

        [SetUp]
        public void Setup()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _synchronizer = CreateArtifactorySynchronizer(Substitute.For<IArtifactorySyncronizationLogger>()); 
        }

        private ArtifactoryGameSynchronizer CreateArtifactorySynchronizer(IArtifactorySyncronizationLogger logger, params PropertySet[] propertySets)
        {
            _internalServices.ArtifactoryGateway.GetAvailablePropertySets().Returns(propertySets);
            return new ArtifactoryGameSynchronizer(_internalServices, logger);
        }

        [Test]
        public void SynchronizeGame_IfSynchronizationIsSuccessfull_SubmitChangesShouldBeCalled()
        {   
            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);
            _internalServices.ArtifactorySynchronizerDataAccessLayer.Received().SubmitChanges();
        }
        
        [Test]
        public void SynchronizeGame_WhenCallingGetGameMethod_ProvidesTheCorectMainGameType()
        {

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(CreateGameTypeDescriptor(130017, "Elm Street"));
            
            

            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            _internalServices.ArtifactorySynchronizerDataAccessLayer.Received().GetGame(130017, Arg.Any<bool>());
            
        }


        [Test]
        public void SynchronizeGame_WhenCallingGetGameNameMethod_ProvidesTheCorectMainGameType()
        {

            _internalServices.ArtifactorySynchronizerDataAccessLayer.GetGameName(130017).Returns("Elm Street");
            _internalServices.ArtifactorySynchronizerDataAccessLayer.GetGameTypes(130017).Returns(Array(CreateGameTypeDescriptor(130017, "Elm Street")));
            
            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            _internalServices.ArtifactorySynchronizerDataAccessLayer.Received().GetGameName(130017);

            
        }


        [Test]
        public void SynchronizeGame_WhenCallingGetGameTypesMethod_ProvidesTheCorectGameType()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.GetGameName(130017).Returns("Elm Street");
            _internalServices.ArtifactorySynchronizerDataAccessLayer.GetGameTypes(130017).Returns(Array(CreateGameTypeDescriptor(130017, "Elm Street")));
            
            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            _internalServices.ArtifactorySynchronizerDataAccessLayer.Received().GetGameTypes(130017);

            
        }

        [Test]
        public void SynchronizeGame_IfIsNewGame_CheckTheNewGameProperties()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(CreateGameTypeDescriptor(130017, "Elm Street"));
            
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g =>
            {
                insertedGame = g;
            }));
            
            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            
            Assert.AreNotEqual(new Guid(), insertedGame.Game_ID, "You forgot to set Game_ID field");
            Assert.AreEqual("Elm Street", insertedGame.GameName);
            Assert.AreEqual(130017, insertedGame.MainGameType);
            Assert.AreEqual(true, insertedGame.IsExternal);
        }

        [Test]
        public void SynchronizeGame_IfIsNewGame_CheckTheGameTypesAdded()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"),
                                                                         new GameTypeDescriptor(9006, OperatorEnum.Bingo, "A nightmare on Elm street PC Bingo"),
                                                                         new GameTypeDescriptor(9057, OperatorEnum.Bingo, "A nightmare on Elm street Mobile Bingo"));
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));

            

            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);
            
            Assert.AreEqual(3, insertedGame.GameTypes.Count);
            var gameType = insertedGame.GameTypes[0];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(130017, gameType.GameType_ID);
            Assert.AreEqual(OperatorEnum.Operator888, (OperatorEnum)gameType.Operator_ID.Value);
            Assert.AreEqual("Elm Street", gameType.Name);

            gameType = insertedGame.GameTypes[1];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(9006, gameType.GameType_ID);
            Assert.AreEqual(OperatorEnum.Bingo, (OperatorEnum)gameType.Operator_ID.Value);
            Assert.AreEqual("A nightmare on Elm street PC Bingo", gameType.Name);


            gameType = insertedGame.GameTypes[2];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(9057, gameType.GameType_ID);
            Assert.AreEqual(OperatorEnum.Bingo, (OperatorEnum)gameType.Operator_ID.Value);
            Assert.AreEqual("A nightmare on Elm street Mobile Bingo", gameType.Name);
        }

        [Test]
        public void SynchronizeGame_AlwaysAddMainGameTypeToTheGameTypesTable()
        {

            _internalServices.ArtifactorySynchronizerDataAccessLayer.GetGameName(130017).Returns("Elm Street");
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));
            
            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            
            Assert.AreEqual(1, insertedGame.GameTypes.Count);
            var gameType = insertedGame.GameTypes[0];
            Assert.AreEqual("Elm Street", gameType.Name);


        }

        [Test]
        public void SynchronizeGame_OnlyMainGameTypeIsAddedForOperator888ToTheListOfGameTypes()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"),
                                                                         new GameTypeDescriptor(130027, OperatorEnum.Operator888, "Elm Street New"),
                                                                         new GameTypeDescriptor(9006, OperatorEnum.Bingo, "A nightmare on Elm street PC Bingo"),
                                                                         new GameTypeDescriptor(9057, OperatorEnum.Bingo, "A nightmare on Elm street Mobile Bingo"));

            
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));


            

            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            
            Assert.AreEqual(3, insertedGame.GameTypes.Count);
            var gameType = insertedGame.GameTypes[0];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(130017, gameType.GameType_ID);
            Assert.AreEqual(0, gameType.Operator_ID);
            Assert.AreEqual("Elm Street", gameType.Name);

            gameType = insertedGame.GameTypes[1];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(9006, gameType.GameType_ID);
            Assert.AreEqual(1, gameType.Operator_ID);
            Assert.AreEqual("A nightmare on Elm street PC Bingo", gameType.Name);


            gameType = insertedGame.GameTypes[2];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(9057, gameType.GameType_ID);
            Assert.AreEqual(1, gameType.Operator_ID);
            Assert.AreEqual("A nightmare on Elm street Mobile Bingo", gameType.Name);
        }



        [TestCase(null)]
        [TestCase("")]
        public void SynchronizeGame_UseTheGameTypeIfGamesDictionaryReturnsNullOrEmptyForTheMainGameTypeName(string gameName)
        {
            
            _internalServices.ArtifactorySynchronizerDataAccessLayer.GetGameName(130017).Returns(gameName);
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));
            
            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            

            Assert.AreEqual(1, insertedGame.GameTypes.Count);
            var gameType = insertedGame.GameTypes[0];
            Assert.AreEqual("130017", gameType.Name);

        }

        [Test]
        public void SynchronizeGame_IfIsExistingGame_CheckIfTheGameNameIsUpdated()
        {
            
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017,  "Elm Street"));
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street (new)"));
            
            

            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            Assert.AreEqual("Elm Street (new)", existingGame.GameName);   
        }


        [Test]
        public void SynchronizeGame_IfIsExistingGame_CheckNewGameTypesAreAdded()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street"));
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame,
                                                                         CreateGameTypeDescriptor(130017, "Elm Street"),
                                                                         CreateGameTypeDescriptor(9006, "A nightmare on Elm street PC Bingo", OperatorEnum.Bingo),
                                                                         CreateGameTypeDescriptor(9057, "A nightmare on Elm street Mobile Bingo", OperatorEnum.Bingo));
            

            

            Assert.AreEqual(1, existingGame.GameTypes.Count);

            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            Assert.AreEqual(3, existingGame.GameTypes.Count);

            var gameType = existingGame.GameTypes[1];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(9006, gameType.GameType_ID);
            Assert.AreEqual(OperatorEnum.Bingo, (OperatorEnum)gameType.Operator_ID.Value);
            Assert.AreEqual("A nightmare on Elm street PC Bingo", gameType.Name);


            gameType = existingGame.GameTypes[2];
            Assert.AreNotEqual(new Guid(), gameType.Row_ID, "You forgot to set Row_ID field");
            Assert.AreEqual(9057, gameType.GameType_ID);
            Assert.AreEqual(OperatorEnum.Bingo, (OperatorEnum)gameType.Operator_ID.Value);
            Assert.AreEqual("A nightmare on Elm street Mobile Bingo", gameType.Name);
        }

        

        [Test]
        public void SynchronizeGame_IfIsExistingGame_CheckGameTypesAreDeleted()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888),
                                                             CreateGameTypeDescriptor(9006, "A nightmare on Elm street PC Bingo", OperatorEnum.Bingo),
                                                             CreateGameTypeDescriptor(9057, "A nightmare on Elm street Mobile Bingo", OperatorEnum.Bingo));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame,
                                                                        CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888),
                                                                        CreateGameTypeDescriptor(9006, "A nightmare on Elm street PC Bingo", OperatorEnum.Bingo));
            
            

            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            Assert.AreEqual(2, existingGame.GameTypes.Count);

            Assert.AreEqual(0, existingGame.GameTypes.Count(gt => gt.GameType_ID == 9057)); 
        }


        [Test]
        public void SynchronizeGame_IfIsExistingGame_CheckGameTypesAreUpdated()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888),
                                                             CreateGameTypeDescriptor(9006, "A nightmare on Elm street PC Bingo", OperatorEnum.Bingo),
                                                             CreateGameTypeDescriptor(9057, "A nightmare on Elm street Mobile Bingo", OperatorEnum.Bingo));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame,
                                                                        CreateGameTypeDescriptor(130017, "Elm Street (new)", OperatorEnum.Operator888),
                                                                        CreateGameTypeDescriptor(9006, "A nightmare on Elm street PC Bingo (new)", OperatorEnum.Bingo),
                                                                        CreateGameTypeDescriptor(9057, "A nightmare on Elm street Mobile Bingo (new)", OperatorEnum.Bingo));

            
            

            _synchronizer.SynchronizeComponent(CreateRepositoryDescriptor(), 130017);

            Assert.AreEqual(3, existingGame.GameTypes.Count);

            var gameType = existingGame.GameTypes[0];
            Assert.AreEqual(130017, gameType.GameType_ID);
            Assert.AreEqual("Elm Street (new)", gameType.Name);

            gameType = existingGame.GameTypes[1];
            Assert.AreEqual(9006, gameType.GameType_ID);
            Assert.AreEqual("A nightmare on Elm street PC Bingo (new)", gameType.Name);


            gameType = existingGame.GameTypes[2];
            Assert.AreEqual(9057, gameType.GameType_ID);
            Assert.AreEqual("A nightmare on Elm street Mobile Bingo (new)", gameType.Name);
        }

        [Test]
        public void SynchronizeGame_WhenCallingTheRepository_GetGameRegulations_ProvidesTheRightMainGameType()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street")));
            
            var repoDescriptor = CreateRepositoryDescriptor();
            
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            repoDescriptor.Repository.Received().GetComponentRegulations(130017);
                
        }


        [Test]
        public void SynchronizeGame_ForEachRegulationShouldCallGetVersionFoldersWithTeCorectParameters()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street")));
            var repoDescriptor = CreateRepositoryDescriptor();
            var actualValues = new List<object>();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, NSubstitute.Arg.Any<string>()).Returns(Array("1.0.0.0", "1.0.0.1"));
            repoDescriptor.Repository.GetArtifact(130017, NSubstitute.Arg.Any<string>(), NSubstitute.Arg.Any<string>()).Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));
            

            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);
            
            repoDescriptor.Repository.Received().GetVersionFolders(130017, "888Italy");
            repoDescriptor.Repository.Received().GetVersionFolders(130017, "Gibraltar");

        }

        [Test]
        public void SynchronizeGame_ForEachRegulationAndVersionShouldCallGetPropertiesWithTeCorectParameters()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street")));
            var repoDescriptor = CreateRepositoryDescriptor();
            

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0", "1.0.0.1"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", NSubstitute.Arg.Any<string>()).Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));
            

            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            repoDescriptor.Repository.Received().GetArtifact(130017, "Gibraltar", "1.0.0.0");
            repoDescriptor.Repository.Received().GetArtifact(130017, "Gibraltar", "1.0.0.1");


        }



        [Test]
        public void SynchronizeGame_NewGameOneRegulationOneVersion_InsertsOneGameVersionRow()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            

            var repoDescriptor = CreateRepositoryDescriptor();
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));
       
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            _internalServices.ArtifactorySynchronizerDataAccessLayer.Received().InsertGame(NSubstitute.Arg.Is<Game>(g => g.GameVersions.Count == 1));
            
        }

        [Test]
        public void SynchronizeGame_IfFileIsMissing_NoVersionIsAdded()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));

            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns((Artifact)null);

      
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(0, insertedGame.GameVersions.Count);
            
        }

        private Artifact CreateStorageItemWithProperties(params ArtifactoryProperty[] properties)
        {
            var artifact = Helpers.RestClientHelpers.CreateArtifact();

            artifact.Properties.AddRange(properties);

            return artifact;
        }


        [Test]
        public void SynchronizeGame_NewGameOneRegulationOneVersion_UseVersionPropertyIfExists()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("version", "0.1.0.0")));


            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);
            _internalServices.ArtifactorySynchronizerDataAccessLayer.Received().InsertGame(Arg.Is<Game>(g => g.GameVersions[0].VersionAsLong == new VersionNumber("0.1.0.0").ToLong()));
        }

        [Test]
        public void SynchronizeGame_NewGameOneRegulationOneVersion_UseVersionFolderIfVersionPropertyDoesntExists()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

         
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            _internalServices.ArtifactorySynchronizerDataAccessLayer.Received().InsertGame(Arg.Is<Game>(g => g.GameVersions[0].VersionAsLong == new VersionNumber("1.0.0.0").ToLong()));

        }


        [Test]
        public void SynchronizeGame_NewGameOneRegulationOneVersion_CheckNewVersionProperties()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));

            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            var storageItem = CreateStorageItemWithProperties(new ArtifactoryProperty("version", "0.1.0.0"), new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "0.1.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(storageItem);
            
            
            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);
            

            Assert.AreEqual(storageItem.createdBy, insertedGame.GameVersions[0].CreatedBy);
            Assert.AreEqual(storageItem.created, insertedGame.GameVersions[0].CreatedDate);
            Assert.AreEqual(insertedGame.Game_ID, insertedGame.GameVersions[0].Game_ID);
            Assert.AreNotEqual(new Guid(), insertedGame.GameVersions[0].GameVersion_ID, "You forgot to set GameVersion_ID field");
            Assert.AreEqual((int)repoDescriptor.Technology, insertedGame.GameVersions[0].Technology);
            Assert.AreEqual("1.0.0.0", insertedGame.GameVersions[0].VersionFolder);
        }


        [Test]
        public void SynchronizeGame_IfTheSameVersionBelongsToMultipleRegulations_InsertItOnlyOnce()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));


            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);
            
            Assert.AreEqual(1, insertedGame.GameVersions.Count);
        }


        [Test]
        public void SynchronizeGame_IfVersionPropertyIsDifferentBetweenRegulations_UseVersionFolder()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));

            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "888Italy").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "888Italy", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "0.1.0.0")));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "0.0.1.0")));

            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);
            
            Assert.AreEqual(new VersionNumber("1.0.0.0").ToLong(), insertedGame.GameVersions[0].VersionAsLong);
        }

        [Test]
        public void SynchronizeGame_IfNoVersionProperty_WriteMessageToLog()
        {
            
            var repoDescriptor = CreateRepositoryDescriptor("modernGame-HTML5");

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, Arg.Any<string>()).Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, Arg.Any<string>(), "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            var logger = ArtifactoryGatewayHelper.CreateMockLogger();
            string actuallMessage = null;
            logger.Info(Arg.Do<string>(msg => actuallMessage = msg));



            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            var synchronizer = CreateArtifactorySynchronizer(logger);

            synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            
            Assert.IsTrue(actuallMessage.Contains("Missing version property"));
            Assert.IsTrue(actuallMessage.Contains("modernGame-HTML5"));
            Assert.IsTrue(actuallMessage.Contains("130017"));
            Assert.IsTrue(actuallMessage.Contains("1.0.0.0"));
            Assert.IsTrue(actuallMessage.Contains("888Italy"));
            Assert.IsTrue(actuallMessage.Contains("Gibraltar"));
            Assert.IsTrue(actuallMessage.Contains("Denmark"));

        }

        [Test]
        public void SynchronizeGame_IfVersionPropertyIsDifferentBetweenRegulations_WriteMessageToLog()
        {
            
            var repoDescriptor = CreateRepositoryDescriptor("modernGame-HTML5");

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, Arg.Any<string>()).Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "888Italy", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "0.1.0.0")));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "0.0.1.0")));
            repoDescriptor.Repository.GetArtifact(130017, "Denmark", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "0.0.1.0")));


            var logger = ArtifactoryGatewayHelper.CreateMockLogger();
            string actuallMessage = null;
            logger.Info(Arg.Do<string>(msg => actuallMessage = msg));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            var synchronizer = CreateArtifactorySynchronizer(logger);

            synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            
            Assert.IsTrue(actuallMessage.Contains("Version property has different values for different regulations"));
            Assert.IsTrue(actuallMessage.Contains("modernGame-HTML5"));
            Assert.IsTrue(actuallMessage.Contains("130017"));
            Assert.IsTrue(actuallMessage.Contains("1.0.0.0; 0.1.0.0 for 888Italy"));
            Assert.IsTrue(actuallMessage.Contains("0.0.1.0 for Gibraltar & Denmark"));
            
        }


        [Test]
        public void SynchronizeGame_DuplicatedVersionFolderShouldNotBeIgnored()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));


            var repoDescriptor = CreateRepositoryDescriptor("modernGame-HTML5");

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.535.0", "1.535.0.0"));

            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.535.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.535.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "1.535.0.0"), new ArtifactoryProperty("NDL.State", "Approved")));
            
            

          
            
            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);
            
            Assert.AreEqual(2, insertedGame.GameVersions.Count);

            var gameVersion = insertedGame.GameVersions[0];
            Assert.AreEqual("1.535.0", gameVersion.VersionFolder);
            Assert.AreEqual(1053500000000, gameVersion.VersionAsLong);

            gameVersion = insertedGame.GameVersions[1];
            Assert.AreEqual("1.535.0.0", gameVersion.VersionFolder);
            Assert.AreEqual(1053500000000, gameVersion.VersionAsLong);
        }

       


        [Test]
        public void SynchronizeGame_ExistingGame_AddNewVersion()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            
            var repoDescriptor = CreateRepositoryDescriptor();
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0", "1.0.0.1"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", Arg.Any<string>()).Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));
            
            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(2, existingGame.GameVersions.Count);
        }

        [Test]
        public void SynchronizeGame_ExistingGame_AddNewVersionWithTheSameNumberButADifferentTechnology()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID, GameTechnology.Flash));
            
            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(2, existingGame.GameVersions.Count);
        }

        [Test]
        public void SynchronizeGame_ExistingGame_RemoveVersion()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.1", existingGame.Game_ID));
            
            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(1, existingGame.GameVersions.Count);
        }

        [Test]
        public void SynchronizeGame_ExistingGame_RemoveVersionOnlyIfDoesntHaveHistory()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID));
            var toRemoveVersinoRecord = GameVersionRecord("1.0.0.1", existingGame.Game_ID);
            toRemoveVersinoRecord.GameVersion_Property_Histories.Add(new GameVersion_Property_History());
            existingGame.GameVersions.Add(toRemoveVersinoRecord);
            
            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(2, existingGame.GameVersions.Count);
        }

        [Test]
        public void SynchronizeGame_ExistingGame_RemoveVersionAccordingWithTheRepositoryGameTechnology()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID, GameTechnology.Flash));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID, GameTechnology.Html5));
            

            var repoDescriptor = CreateRepositoryDescriptor("modernGame-local", GameTechnology.Flash);

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(new string[0]);
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(1, existingGame.GameVersions.Count);
            Assert.AreEqual((int)GameTechnology.Html5, existingGame.GameVersions[0].Technology);
        }

        [Test]
        public void SynchronizeGame_ExistingGame_UpdateLongVersion()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID));
            

            var repoDescriptor = CreateRepositoryDescriptor("modernGame-local");

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty(WellKnownPropertiesNames.VersionProperty, "0.1.0.0")));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(1, existingGame.GameVersions.Count);
            Assert.AreEqual(new VersionNumber("0.1.0.0").ToLong(), existingGame.GameVersions[0].VersionAsLong);
            
        }

        [Test]
        public void SynchronizeGame_ExistingGame_IfTheVersionHasNoArtifactsThenRemoveIt()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID));
            

            var repoDescriptor = CreateRepositoryDescriptor("modernGame-local");

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns((Artifact)null);

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(0, existingGame.GameVersions.Count);
        }


        [Test]
        public void SynchronizeGame_SyncProps_OneRegulation_OneNewProperty()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            var existingGameVersion = GameVersionRecord("1.0.0.0", existingGame.Game_ID);
            existingGame.GameVersions.Add(existingGameVersion);
            
            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(1, existingGameVersion.GameVersion_Properties.Count);

            var propertyRow = existingGameVersion.GameVersion_Properties[0];
            Assert.AreEqual("NDL.State", propertyRow.PropertyKey);
            Assert.AreEqual("NDL", propertyRow.PropertySet);
            Assert.AreEqual("State", propertyRow.PropertyName);
            Assert.AreEqual("InProgress", propertyRow.PropertyValue);
            Assert.AreEqual("Gibraltar", propertyRow.Regulation);
            Assert.AreEqual(existingGameVersion.GameVersion_ID, propertyRow.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propertyRow.Row_ID);
        }

        [Test]
        public void SynchronizeGame_SyncProps_TwoRegulations_OneNewProperty()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            var existingGameVersion = GameVersionRecord("1.0.0.0", existingGame.Game_ID);
            existingGame.GameVersions.Add(existingGameVersion);


            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));

            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Denmark").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));
            repoDescriptor.Repository.GetArtifact(130017, "Denmark", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("DL.State", "Approved")));

            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(2, existingGameVersion.GameVersion_Properties.Count);

            var propertyRow = existingGameVersion.GameVersion_Properties[0];
            Assert.AreEqual("NDL.State", propertyRow.PropertyKey);
            Assert.AreEqual("NDL", propertyRow.PropertySet);
            Assert.AreEqual("State", propertyRow.PropertyName);
            Assert.AreEqual("InProgress", propertyRow.PropertyValue);
            Assert.AreEqual("Gibraltar", propertyRow.Regulation);
            Assert.AreEqual(existingGameVersion.GameVersion_ID, propertyRow.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propertyRow.Row_ID);

            propertyRow = existingGameVersion.GameVersion_Properties[1];
            Assert.AreEqual("DL.State", propertyRow.PropertyKey);
            Assert.AreEqual("DL", propertyRow.PropertySet);
            Assert.AreEqual("State", propertyRow.PropertyName);
            Assert.AreEqual("Approved", propertyRow.PropertyValue);
            Assert.AreEqual("Denmark", propertyRow.Regulation);
            Assert.AreEqual(existingGameVersion.GameVersion_ID, propertyRow.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propertyRow.Row_ID);

            Assert.AreNotEqual(existingGameVersion.GameVersion_Properties[0].Row_ID, existingGameVersion.GameVersion_Properties[1].Row_ID);

        }

        [Test]
        public void SynchronizeGame_SyncProps_OneRegulations_OnePropertyRemoved()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            var existingGameVersion = GameVersionRecord("1.0.0.0", 
                                                              existingGame.Game_ID, 
                                                              GameTechnology.Html5,
                                                              new KeyValuePair<string, ArtifactoryProperty[]>("Gibraltar", Array(new ArtifactoryProperty("NDL.State", "InProgress"), 
                                                                                                                                       new ArtifactoryProperty("DL.State", "Approved"))));
            
            existingGame.GameVersions.Add(existingGameVersion);
            

            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));


            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(1, existingGameVersion.GameVersion_Properties.Count);

            var propertyRow = existingGameVersion.GameVersion_Properties[0];
            Assert.AreEqual("NDL.State", propertyRow.PropertyKey);
            Assert.AreEqual("NDL", propertyRow.PropertySet);
            Assert.AreEqual("State", propertyRow.PropertyName);
            Assert.AreEqual("InProgress", propertyRow.PropertyValue);
            Assert.AreEqual("Gibraltar", propertyRow.Regulation);
            Assert.AreEqual(existingGameVersion.GameVersion_ID, propertyRow.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propertyRow.Row_ID);


        }

        [Test]
        public void SynchronizeGame_SyncProps_OneRegulation_OnePropertyUpdated()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            var existingGameVersion = GameVersionRecord("1.0.0.0",
                                                              existingGame.Game_ID,
                                                              GameTechnology.Html5,
                                                              new KeyValuePair<string, ArtifactoryProperty[]>("Gibraltar", Array(new ArtifactoryProperty("NDL.State", "InProgress"))));

            existingGame.GameVersions.Add(existingGameVersion);
            

            var repoDescriptor = CreateRepositoryDescriptor();
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "Approved")));


            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(1, existingGameVersion.GameVersion_Properties.Count);

            var propertyRow = existingGameVersion.GameVersion_Properties[0];
            Assert.AreEqual("NDL.State", propertyRow.PropertyKey);
            Assert.AreEqual("NDL", propertyRow.PropertySet);
            Assert.AreEqual("State", propertyRow.PropertyName);
            Assert.AreEqual("Approved", propertyRow.PropertyValue);
            Assert.AreEqual("Gibraltar", propertyRow.Regulation);
            Assert.AreEqual(existingGameVersion.GameVersion_ID, propertyRow.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propertyRow.Row_ID);
        }


        [Test]
        public void SynchronizeGame_ExistingGame_IfDownloadUrlDontExists_AddThem()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID));
                       

            var repoDescriptor = CreateRepositoryDescriptor();
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Denmark").Returns(Array("1.0.0.0"));
            var a1 = CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress"));
            a1.downloadUri = "http://artifactory/RepoName/Gibraltar/1.0.0.0/file.zip";
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(a1);

            var a2 = CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress"));
            a2.downloadUri = "http://artifactory/RepoName/Denmark/1.0.0.0/file.zip";
            repoDescriptor.Repository.GetArtifact(130017, "Denmark", "1.0.0.0").Returns(a2);


            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(2, existingGame.GameVersions[0].GameVersion_Regulations.Count);
            var uriRow = existingGame.GameVersions[0].GameVersion_Regulations.First(row => row.Regulation == "Gibraltar");
            Assert.AreEqual("http://artifactory/RepoName/Gibraltar/1.0.0.0/file.zip", uriRow.DownloadUri);
            Assert.AreEqual("file.zip", uriRow.FileName);

            uriRow = existingGame.GameVersions[0].GameVersion_Regulations.First(row => row.Regulation == "Denmark");
            Assert.AreEqual("http://artifactory/RepoName/Denmark/1.0.0.0/file.zip", uriRow.DownloadUri);
            Assert.AreEqual("file.zip", uriRow.FileName);
        }

        [Test]
        public void SynchronizeGame__ExistingGame_IfDownloadUrlsExists_UpdateThem()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            existingGame.GameVersions.Add(GameVersionRecord("1.0.0.0", existingGame.Game_ID));
            var downloadUrls = existingGame.GameVersions[0];
            downloadUrls.GameVersion_Regulations.Add(new GameVersion_Regulation()
                    {
                        Row_ID = Guid.NewGuid(), 
                        DownloadUri = "http://artifactory/RepoName/Gibraltar/1.0.0.0/file.zip",
                        GameVersion_ID = existingGame.GameVersions[0].GameVersion_ID,
                        Regulation = "Gibraltar"
                    });

            downloadUrls.GameVersion_Regulations.Add(new GameVersion_Regulation()
            {
                Row_ID = Guid.NewGuid(),
                DownloadUri = "http://artifactory/RepoName/Denmark/1.0.0.0/file.zip",
                GameVersion_ID = existingGame.GameVersions[0].GameVersion_ID,
                Regulation = "Denmark"
            });

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));
            
            var repoDescriptor = CreateRepositoryDescriptor();

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Denmark").Returns(Array("1.0.0.0"));


            var a1 = CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress"));
            a1.downloadUri = "http://artifactory/RepoName/Gibraltar/1.0.0.0/file2.zip";
            var a2 = CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress"));
            a2.downloadUri = "http://artifactory/RepoName/Denmark/1.0.0.0/file2.zip";

            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(a1);
            repoDescriptor.Repository.GetArtifact(130017, "Denmark", "1.0.0.0").Returns(a2);
            

            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(2, existingGame.GameVersions[0].GameVersion_Regulations.Count);

            var uriRow = existingGame.GameVersions[0].GameVersion_Regulations.First(row => row.Regulation == "Gibraltar");
            Assert.AreEqual("http://artifactory/RepoName/Gibraltar/1.0.0.0/file2.zip", uriRow.DownloadUri);
            Assert.AreEqual("file2.zip", uriRow.FileName);

            uriRow = existingGame.GameVersions[0].GameVersion_Regulations.First(row => row.Regulation == "Denmark");
            Assert.AreEqual("http://artifactory/RepoName/Denmark/1.0.0.0/file2.zip", uriRow.DownloadUri);
            Assert.AreEqual("file2.zip", uriRow.FileName);
        }


        [Test]
        public void SynchronizeGame_NewGameVersion_AddDownloadUrls()
        {
            var existingGame = GameRecord(130017, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));

            Assert.AreEqual(0, existingGame.GameVersions.Count);

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(existingGame, CreateGameTypeDescriptor(130017, "Elm Street", OperatorEnum.Operator888));

            var repoDescriptor = CreateRepositoryDescriptor();
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Denmark").Returns(Array("1.0.0.0"));


            var a1 = CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress"));
            a1.downloadUri = "http://artifactory/RepoName/Gibraltar/1.0.0.0/file.zip";
            var a2 = CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress"));
            a2.downloadUri = "http://artifactory/RepoName/Denmark/1.0.0.0/file.zip";

            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(a1);
            repoDescriptor.Repository.GetArtifact(130017, "Denmark", "1.0.0.0").Returns(a2);
            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            Assert.AreEqual(2, existingGame.GameVersions[0].GameVersion_Regulations.Count);

            var uriRow = existingGame.GameVersions[0].GameVersion_Regulations.First(row => row.Regulation == "Gibraltar");
            Assert.AreEqual("http://artifactory/RepoName/Gibraltar/1.0.0.0/file.zip", uriRow.DownloadUri);
            Assert.AreEqual("file.zip", uriRow.FileName);

            uriRow = existingGame.GameVersions[0].GameVersion_Regulations.First(row => row.Regulation == "Denmark");
            Assert.AreEqual("http://artifactory/RepoName/Denmark/1.0.0.0/file.zip", uriRow.DownloadUri);
            Assert.AreEqual("file.zip", uriRow.FileName);
        }


        [Test]
        public void SynchronizeGame_ShouldCall_GamesPortalHubContext_GameSynchronizationFinished_IfThereAreAnyChangesForThatGame()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.HasChanges.Returns(true);
            Game insertedGame = null;
            _internalServices.ArtifactorySynchronizerDataAccessLayer.InsertGame(Arg.Do<Game>(g => insertedGame = g));

            var repoDescriptor = CreateRepositoryDescriptor();
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));


            
            
            SignalR.GameSynchronizationFinishedData actualGameSynchronizationFinishedData = null;
            _internalServices.GamesPortalHubContext.GameSynchronizationFinished(Arg.Do<SignalR.GameSynchronizationFinishedData>(x => actualGameSynchronizationFinishedData = x));


            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);
            
            
            Assert.AreEqual(insertedGame.Game_ID, actualGameSynchronizationFinishedData.GameId);
        }


        [Test]
        public void SynchronizeGame_ShouldNOTCall_GamesPortalHubContext_GameSynchronizationFinished_IfThereAreNoChangesForThatGame()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));

            _internalServices.ArtifactorySynchronizerDataAccessLayer.HasChanges.Returns(false);
            
            var repoDescriptor = CreateRepositoryDescriptor();
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("Gibraltar"));
            repoDescriptor.Repository.GetVersionFolders(130017, "Gibraltar").Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, "Gibraltar", "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "InProgress")));

            
            

            _synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            _internalServices.GamesPortalHubContext.DidNotReceive().GameSynchronizationFinished(Arg.Any<SignalR.GameSynchronizationFinishedData>());
        }


       

        [TestCase(PropertyType.SINGLE_SELECT)]
        [TestCase(PropertyType.MULTI_SELECT)]
        public void CheckPropertyValue_IfPropertyValueIsNotWhatWeExpect_WriteToLog(PropertyType propertyType)
        {
            
            var repoDescriptor = CreateRepositoryDescriptor("modernGame-HTML5");
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, Arg.Any<string>()).Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, Arg.Any<string>(), "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "XXX")));

            var logger = ArtifactoryGatewayHelper.CreateMockLogger();

            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            var synchronizer = CreateArtifactorySynchronizer(logger,
                                                             new PropertySet("NDL", new PropertyDefinition("State", 
                                                                                                           propertyType,
                                                                                                           new PropertyPredefinedValue("InProgress", true),
                                                                                                           new PropertyPredefinedValue("Approved", true))));

            string actuallMessage = null;
            logger.Warn(Arg.Do<string>(msg => actuallMessage = msg));

            synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            


            Assert.IsTrue(actuallMessage.Contains("NDL.State"));
            Assert.IsTrue(actuallMessage.Contains("XXX"));
        }


        [Test]
        public void CheckPropertyValue_IfPropertyIsConfiguredAsIgnored_DontWriteAnyWarnToLogToLog()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));

            var repoDescriptor = CreateRepositoryDescriptor("modernGame-HTML5");
            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, Arg.Any<string>()).Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, Arg.Any<string>(), "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "XXX")));


            
            var logger = Substitute.For<IArtifactorySyncronizationLogger>();
            
            var synchronizer = CreateArtifactorySynchronizer(logger, 
                                                            new PropertySet("NDL", new PropertyDefinition("State", PropertyType.SINGLE_SELECT,
                                                                                    new PropertyPredefinedValue("InProgress", true),
                                                                                    new PropertyPredefinedValue("Approved", true))));

            synchronizer.IgnoredUndefinedPropertyValue("NDL.State", "XXX");
            
            synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            logger.DidNotReceive().Warn(Arg.Any<string>());
        }


        [Test]
        public void CheckPropertyValue_IfPropertyTypeIsAny_DontWriteAnyWarnToLogToLog()
        {
            _internalServices.ArtifactorySynchronizerDataAccessLayer.AddMockData(new GameTypeDescriptor(130017, OperatorEnum.Operator888, "Elm Street"));
            
            var repoDescriptor = CreateRepositoryDescriptor("modernGame-HTML5");

            repoDescriptor.Repository.GetComponentRegulations(130017).Returns(Array("888Italy", "Gibraltar", "Denmark"));
            repoDescriptor.Repository.GetVersionFolders(130017, Arg.Any<string>()).Returns(Array("1.0.0.0"));
            repoDescriptor.Repository.GetArtifact(130017, Arg.Any<string>(), "1.0.0.0").Returns(CreateStorageItemWithProperties(new ArtifactoryProperty("NDL.State", "XXX")));

            

            var logger = Substitute.For<IArtifactorySyncronizationLogger>();
                       
            var synchronizer = CreateArtifactorySynchronizer(logger, 
                                                             new PropertySet("NDL", new PropertyDefinition("State", PropertyType.ANY_VALUE,
                                                             new PropertyPredefinedValue("InProgress", true),
                                                             new PropertyPredefinedValue("Approved", true))));

            synchronizer.SynchronizeComponent(repoDescriptor, 130017);

            logger.DidNotReceive().Warn(Arg.Any<string>());
        }
      
    }
}
