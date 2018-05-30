using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Artifactory;
using GamesPortal.Service.DataAccessLayer;
using GamesPortal.Service.Entities;
using GamesPortal.Service.Helpers;
using Spark.Infra.Types;
using Microsoft.Practices.Unity;
using NSubstitute;
using NUnit.Framework;


namespace GamesPortal.Service
{
    
    public abstract class GamesPortalApprovalService_ApproveTests<TApproveRequest>
        where TApproveRequest : ApproveRequestBase
    {

        protected abstract string GetPropertyValue();
        protected abstract string GetPropertyName();
        protected abstract TApproveRequest CreateApproveRequest();
        protected abstract TApproveRequest CreateApproveRequest(Guid gameVersionId, params string[] regulations);
        
        protected abstract void Approve(GamesPortalApprovalService service, TApproveRequest request);


        [SetUp]
        public void Setup()
        {
            _internalServices = GamesPortalInternalServicesHelper.Create();
            _gamesPortalApprovalService = new GamesPortalApprovalService(_internalServices);
        }

        protected IGamesPortalInternalServices _internalServices;
        protected GamesPortalApprovalService _gamesPortalApprovalService;

     
        [Test]
        public void Approve_ShouldGetUserNameFromSecurityService()
        {

            _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Html5);
            _internalServices.CallContextServices.GetCallingUserName().Returns("Florin");
            
            
            
            Guid versionID = Guid.NewGuid();

            _internalServices.CreateGamesPortalDBDataContext().MockTable(CreateGameVersionRecord(versionID));
            

            Approve(_gamesPortalApprovalService, CreateApproveRequest(versionID));

            _internalServices.CallContextServices.Received().GetCallingUserName();
            
        }

        [Test]
        public void Approve_IfGameVersionIsNotFound_ThrowArgumentException()
        {
            
            _internalServices.CreateGamesPortalDBDataContext().MockTable(CreateGameVersionRecord(Guid.NewGuid()));

            

            Assert.Throws<ArgumentException>(() => Approve(_gamesPortalApprovalService, CreateApproveRequest()));
        }

        [Test]
        public void Approve_IfGameVersionIsFound_ShouldNotThrowArgumentException()
        {
            Guid versionID = Guid.NewGuid();

            _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Html5);
            _internalServices.CreateGamesPortalDBDataContext().MockTable(CreateGameVersionRecord(versionID));
            
            

            Approve(_gamesPortalApprovalService, CreateApproveRequest(versionID));
        }


        [TestCase(GameTechnology.Flash, PlatformType.Mobile,  false, GamingComponentCategory.Game)]
        [TestCase(GameTechnology.Html5, PlatformType.PC, false, GamingComponentCategory.Game)]
        [TestCase(GameTechnology.Flash, PlatformType.PC, true, GamingComponentCategory.Game)]
        [TestCase(GameTechnology.Html5, PlatformType.Mobile, true, GamingComponentCategory.Game)]
        [TestCase(GameTechnology.Html5, PlatformType.Mobile, false, GamingComponentCategory.Chill)]
        [TestCase(GameTechnology.Flash, PlatformType.PC, false, GamingComponentCategory.Wrapper)]
        public void Approve_WhenCallingArtifactoryGatway_GetRepositoryDescriptor_ProvideTheCorrectArguments(GameTechnology expectedGameTehnology, PlatformType expectedPlatformType, bool expectedIsExternal, GamingComponentCategory expectedCategory)
        {


            GameInfrastructureDTO actualGameInfrastructure = null;
            bool? actualIsExternal = null;
            GamingComponentCategory? actualCategory = null;

            _internalServices.ArtifactorySynchronizationManager.MockGameRepository(expectedGameTehnology, expectedIsExternal);
            _internalServices.ArtifactorySynchronizationManager.FindRepositoryDescriptor(Arg.Do<GameInfrastructureDTO>(arg => actualGameInfrastructure = arg),
                                                                 Arg.Do<bool>(arg => actualIsExternal = arg),
                                                                 Arg.Do<GamingComponentCategory>(arg => actualCategory = arg));

            

            Guid versionID = Guid.NewGuid();

            var gameVersionRecord = CreateGameVersionRecord(versionID);
            gameVersionRecord.Technology = (int)expectedGameTehnology;
            gameVersionRecord.PlatformType = (int)expectedPlatformType;
            gameVersionRecord.Game.IsExternal = expectedIsExternal;
            gameVersionRecord.Game.ComponentCategory = (int)expectedCategory;

            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "888Italy"));
            

            _internalServices.CreateGamesPortalDBDataContext().MockTable(gameVersionRecord);

            

            Approve(_gamesPortalApprovalService, CreateApproveRequest(versionID, "888Italy"));

            Assert.AreEqual(expectedGameTehnology, actualGameInfrastructure.GameTechnology);
            Assert.AreEqual(expectedPlatformType, actualGameInfrastructure.PlatformType);
            Assert.AreEqual(expectedIsExternal, actualIsExternal.Value);
            Assert.AreEqual(expectedCategory, actualCategory);
        }


        [Test]
        public void Approve_WhenCallingArtifactoryRepository_UpdateArtifactProperties_ProvideTheCorrectArgumentsForIdentifyingTheArtifact()
        {

            var gameTechnology = GameTechnology.Flash;
            var isExternal = true;


            
            var gamesRepositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(gameTechnology, isExternal);

            UpdateArtifactPropertiesRequest actualRequest = null;

            gamesRepositoryDescriptor.Repository.UpdateArtifactProperties(Arg.Do<UpdateArtifactPropertiesRequest>(arg => actualRequest = arg));

         
            Guid versionID = Guid.NewGuid();
            
            var gameVersionRecord = CreateGameVersionRecord(versionID);
            gameVersionRecord.Game.MainGameType = 130017;
            gameVersionRecord.Game.IsExternal = isExternal;

            gameVersionRecord.Technology = (int)gameTechnology;
            gameVersionRecord.VersionFolder = "1.2.3";
            gameVersionRecord.VersionAsLong = VersionNumber.Parse("0.1.2.3").ToLong();

            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "Gibraltar"));

            _internalServices.CreateGamesPortalDBDataContext().MockTable(gameVersionRecord);

            

            Approve(_gamesPortalApprovalService, CreateApproveRequest(versionID, "Gibraltar"));

            Assert.AreEqual(130017, actualRequest.ComponentId);
            Assert.AreEqual("Gibraltar", actualRequest.Regulation);
            Assert.AreEqual("1.2.3", actualRequest.Version);
        }


        [Test]
        public void Approve_ShouldCall_UpdateArtifactProperties_ForEachRegulationInRequest()
        {

            

            var gamesRepositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash);

            var actualRegulations = new List<string>();



            gamesRepositoryDescriptor.Repository.UpdateArtifactProperties(Arg.Do<UpdateArtifactPropertiesRequest>(arg => actualRegulations.Add(arg.Regulation)));

        
            Guid versionID = Guid.NewGuid();

            var gameVersionRecord = CreateGameVersionRecord(versionID);
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "Gibraltar"));
            _internalServices.CreateGamesPortalDBDataContext().MockTable(gameVersionRecord);

            

            var request = new QAApproveRequest(versionID,
                                               "888Italy", "Gibraltar");

            _gamesPortalApprovalService.QAApprove(request);

            Assert.AreEqual(2, actualRegulations.Count);
            Assert.IsTrue(actualRegulations.Any(r => r == "888Italy"));
            Assert.IsTrue(actualRegulations.Any(r => r == "Gibraltar"));
        }

        [Test]
        public void Approve_ShouldCall_UpdateArtifactProperties_And_ProvideTheCorrectPropertiesForEachRegulation()
        {
            

            var gamesRepositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash);

            var actualProperties = new Dictionary<string, ArtifactoryProperty[]>();

            gamesRepositoryDescriptor.Repository.UpdateArtifactProperties(Arg.Do<UpdateArtifactPropertiesRequest>(arg => actualProperties.Add(arg.Regulation, arg.Properties)));

     
            Guid versionID = Guid.NewGuid();


            var gameVersionRecord = CreateGameVersionRecord(versionID);
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "Gibraltar"));
            _internalServices.CreateGamesPortalDBDataContext().MockTable(gameVersionRecord);

            

            var request = CreateApproveRequest(versionID,
                                               "888Italy", "Gibraltar");

            Approve(_gamesPortalApprovalService, request);

            var propertiesPerRegulation = actualProperties["888Italy"];
            Assert.AreEqual(1, propertiesPerRegulation.Length);
            var prop = propertiesPerRegulation.First(p => p.Key == GetPropertyName());
            Assert.AreEqual(GetPropertyValue(), prop.ConcatValues());


            propertiesPerRegulation = actualProperties["Gibraltar"];
            Assert.AreEqual(1, propertiesPerRegulation.Length);
            prop = propertiesPerRegulation.First(p => p.Key == GetPropertyName());
            Assert.AreEqual(GetPropertyValue(), prop.ConcatValues());

        }

        [Test]
        public void Approve_ShouldInsertTheAccordingRecordsInThe_GameVersion_Property_Table()
        {
            _internalServices.CallContextServices.GetCallingUserName().Returns("florin");
            
            var gamesRepositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash);



            Guid versionID = Guid.NewGuid();

            

            var gameVersionRecord = CreateGameVersionRecord(versionID);
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "Gibraltar"));

            _internalServices.CreateGamesPortalDBDataContext().MockTable(gameVersionRecord);



            var request = CreateApproveRequest(versionID,
                                               "888Italy", "Gibraltar");

            Approve(_gamesPortalApprovalService, request);

            var propertiesTable = gameVersionRecord.GameVersion_Properties;
            Assert.AreEqual(2, propertiesTable.Count);
            var propRecord = propertiesTable.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "888Italy");

            Assert.AreEqual(GetPropertyValue(), propRecord.PropertyValue);
            Assert.IsNull(propRecord.PropertySet);
            Assert.AreEqual(GetPropertyName(), propRecord.PropertyName);
            Assert.AreEqual("florin", propRecord.ChangedBy);
            Assert.AreNotEqual(DateTime.MinValue, propRecord.LastChange);
            Assert.AreEqual(gameVersionRecord.GameVersion_ID, propRecord.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propRecord.Row_ID);

            propRecord = propertiesTable.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "Gibraltar");
            Assert.AreEqual(GetPropertyValue(), propRecord.PropertyValue);
            Assert.IsNull(propRecord.PropertySet);
            Assert.AreEqual(GetPropertyName(), propRecord.PropertyName);
            Assert.AreEqual("florin", propRecord.ChangedBy);
            Assert.AreNotEqual(DateTime.MinValue, propRecord.LastChange);
            Assert.AreEqual(gameVersionRecord.GameVersion_ID, propRecord.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propRecord.Row_ID);
        }



        [Test]
        public void Approve_ShouldInsertOnlyNewPropertiesTheAccordingRecordsInThe_GameVersion_Property_Table()
        {

            _internalServices.CallContextServices.GetCallingUserName().Returns("florin");
            
            _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash);
            
            Guid versionID = Guid.NewGuid();
            
            var gameVersionRecord = CreateGameVersionRecord(versionID);
            var previousLastChange = DateTime.Now.AddYears(-1);
            var originalRowID = Guid.NewGuid();

            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "Gibraltar"));

            var gameVersionPropertyRecord = MockRecordsFactory.GameVersionPropertyRecord(versionID, GetPropertyName(), WellKnownNamesAndValues.True, "888Italy", previousLastChange, "adrian");
            gameVersionPropertyRecord.Row_ID = originalRowID;

            gameVersionRecord.GameVersion_Properties.Add(gameVersionPropertyRecord);

            _internalServices.CreateGamesPortalDBDataContext().MockTable(gameVersionRecord);



            var request = CreateApproveRequest(versionID,
                                               "888Italy", "Gibraltar");

            Approve(_gamesPortalApprovalService, request);

            var propertiesTable = gameVersionRecord.GameVersion_Properties;
            Assert.AreEqual(2, propertiesTable.Count);

            var propRecord = propertiesTable.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "888Italy");
            Assert.AreEqual(GetPropertyValue(), propRecord.PropertyValue);
            Assert.IsNull(propRecord.PropertySet);
            Assert.AreEqual(GetPropertyName(), propRecord.PropertyName);
            Assert.AreEqual("florin", propRecord.ChangedBy);
            Assert.AreNotEqual(previousLastChange, propRecord.LastChange);
            Assert.AreEqual(gameVersionRecord.GameVersion_ID, propRecord.GameVersion_ID);
            Assert.AreEqual(originalRowID, propRecord.Row_ID);

            propRecord = propertiesTable.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "Gibraltar");
            Assert.AreEqual(GetPropertyValue(), propRecord.PropertyValue);
            Assert.IsNull(propRecord.PropertySet);
            Assert.AreEqual(GetPropertyName(), propRecord.PropertyName);
            Assert.AreEqual("florin", propRecord.ChangedBy);
            Assert.AreNotEqual(DateTime.MinValue, propRecord.LastChange);
            Assert.AreEqual(gameVersionRecord.GameVersion_ID, propRecord.GameVersion_ID);
            Assert.AreNotEqual(new Guid(), propRecord.Row_ID);
        }


        [Test]
        public void Approve_ShouldAddRecordsToThe_GameVersion_Properties_History_Table()
        {
            _internalServices.CallContextServices.GetCallingUserName().Returns("florin");
            
            _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash);
            
            Guid versionID = Guid.NewGuid();
            
            var gameVersionRecord = CreateGameVersionRecord(versionID);
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "888Italy"));
            gameVersionRecord.GameVersion_Regulations.Add(MockRecordsFactory.GameVersion_RegulationRecord(Guid.NewGuid(), "Gibraltar"));



            gameVersionRecord.GameVersion_Properties.Add(MockRecordsFactory.GameVersionPropertyRecord(versionID, GetPropertyName(), WellKnownNamesAndValues.False, "888Italy", "adrian"));

            _internalServices.CreateGamesPortalDBDataContext().MockTable(gameVersionRecord);



            var request = CreateApproveRequest(versionID,
                                               "888Italy", "Gibraltar");

            Approve(_gamesPortalApprovalService, request);

            var historyTable = gameVersionRecord.GameVersion_Property_Histories;
            Assert.AreEqual(2, historyTable.Count);

            var propertyRecord = gameVersionRecord.GameVersion_Properties.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "888Italy");
            var historyRecord = historyTable.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "888Italy");
            Assert.AreEqual(propertyRecord.LastChange, historyRecord.ChangeDate);
            Assert.AreEqual((int)RecordChangeType.Changed, historyRecord.ChangeType);
            Assert.AreEqual("florin", historyRecord.ChangedBy);
            Assert.AreEqual(gameVersionRecord.GameVersion_ID, historyRecord.GameVersion_ID);
            Assert.AreEqual(GetPropertyValue(), historyRecord.NewValue);
            Assert.AreEqual(WellKnownNamesAndValues.False, historyRecord.OldValue);


            propertyRecord = gameVersionRecord.GameVersion_Properties.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "Gibraltar");
            historyRecord = historyTable.First(p => p.PropertyKey == GetPropertyName() && p.Regulation == "Gibraltar");
            Assert.AreEqual(propertyRecord.LastChange, historyRecord.ChangeDate);
            Assert.AreEqual((int)RecordChangeType.Added, historyRecord.ChangeType);
            Assert.AreEqual("florin", historyRecord.ChangedBy);
            Assert.AreEqual(gameVersionRecord.GameVersion_ID, historyRecord.GameVersion_ID);
            Assert.AreEqual(GetPropertyValue(), historyRecord.NewValue);
            Assert.IsNull(historyRecord.OldValue);
        }

        [Test]
        public void Approve_ShouldSubmitChangesToTheDB()
        {
            
            var gamesRepositoryDescriptor = _internalServices.ArtifactorySynchronizationManager.MockGameRepository(GameTechnology.Flash);

            Guid versionID = Guid.NewGuid();
            
            _internalServices.CreateGamesPortalDBDataContext().MockTable(CreateGameVersionRecord(versionID));

            
            
            Approve(_gamesPortalApprovalService, CreateApproveRequest(versionID));

            _internalServices.CreateGamesPortalDBDataContext().Received().SubmitChanges();
            
        }


        protected static GameVersion CreateGameVersionRecord(Guid versionID, GameTechnology gameTechnology = GameTechnology.Html5, bool isExternal = false, params GameVersion_Property[] properties)
        {

            var gameVersionRecord = new GameVersion()
            {
                GameVersion_ID = versionID,
                Game = new Game() { Game_ID = Guid.NewGuid() }
            };
            gameVersionRecord.Technology = (int)gameTechnology;
            gameVersionRecord.PlatformType = (int)ArtifactoryHelper.DEFAULT_PLATFORM_TYPE;
            gameVersionRecord.VersionFolder = "1.2.3";
            gameVersionRecord.VersionAsLong = VersionNumber.Parse("0.1.2.3").ToLong();
            gameVersionRecord.Game.IsExternal = isExternal;
            gameVersionRecord.Game.MainGameType = 130017;
            gameVersionRecord.Game.ComponentCategory = (int)GamingComponentCategory.Game;


            gameVersionRecord.GameVersion_Properties.AddRange(properties ?? new GameVersion_Property[0]);

            return gameVersionRecord;
        }

        


    }
}
