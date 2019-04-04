using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using NSubstitute;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Types;
using Spark.Infra;
using Spark.Infra.Logging;

namespace GamesPortal.Service.Artifactory
{

    [TestFixture]
    public class ComponentVersionRecordBuilderTests
    {

        IGamesPortalInternalServices _internalServices;
        IArtifactorySyncronizationLogger _logger;
        IComponentVersionBuilder _builder;
        IGamesRepository _repository;
        GamesRepositoryDescriptor _repoDescriptor;
        const int _gameType = 130017;
        readonly Guid _gameId = new Guid("68A00500-1247-4FDA-A3FE-BFE036064497");

        const string _version = "1.0.3.1";


        [SetUp]
        public void Setup()
        {
            _logger = Substitute.For<IArtifactorySyncronizationLogger>();
            _internalServices = Helpers.GamesPortalInternalServicesHelper.Create();

            Init(BuildRegulationArtifact("Gibraltar"));
        }



        private void Init(params RegulationArtifact[] artifactsPerRegulations)
        {
            InitRepository(artifactsPerRegulations);
            InitBuilder();
        }

        private RegulationArtifact BuildRegulationArtifact(string regulation, string withVersionProperty = "")
        {
            var artifact = new Artifact();
            if (!string.IsNullOrEmpty(withVersionProperty))
            {
                artifact.Properties = new ArtifactoryPropertyCollection()
                                        {
                                            new ArtifactoryProperty(WellKnownNamesAndValues.VersionProperty, withVersionProperty)
                                        };
            }

            artifact.downloadUri = $"http://artifactory/{regulation}/file.zip";
            artifact.created = DateTime.Now;
            artifact.createdBy = "florin";
            artifact.size = 100;
            artifact.checksums = new Artifact.ArtifactChecksums(Guid.NewGuid().ToString(), Guid.NewGuid().ToString());

            return new RegulationArtifact(regulation, artifact);
        }
        
        private void InitRepository(params RegulationArtifact[] regulationArtifacts)
        {
            _repository = Substitute.For<IGamesRepository>();
            _repository.GetComponentRegulations(_gameType).Returns(regulationArtifacts.Select(a => a.Regulation).ToArray());
            _repository.RepositoryName.Returns("repo");
            _repository.GetRootFolderRelativeUrl().Returns("repo/games");

            foreach(var regulationArtifact in regulationArtifacts)
            {
                _repository.GetArtifact(_gameType, regulationArtifact.Regulation, _version).Returns(Optional<Artifact>.Some(regulationArtifact.Artifact));
            }

            _repoDescriptor = new GamesRepositoryDescriptor(Entities.GameTechnology.Html5, false, Entities.PlatformType.PcAndMobile, _repository);
        }

        private void InitBuilder()
        {
            _builder = ComponentVersionRecordBuilder.Init(_logger, _internalServices)
                                        .FromRepository(_repoDescriptor)
                                        .WithComponentType(130017)
                                        .WithComponentId(_gameId)
                                        .WithVersionFolder(_version);
        }

        [Test]
        public void Build_ShouldCallRepository_GetRegulations_WithTheCorrectArgument()
        {
            _builder.Build();
            _repository.Received(1).GetComponentRegulations(_gameType);
        }


        [Test]
        public void Build_ForEachRegulation_ShouldCallGetArtifactWithTheProvidedVersion()
        {
            Init(BuildRegulationArtifact("Gibraltar"), 
                 BuildRegulationArtifact("888Italy"));

            
            _builder.Build();

            _repository.Received().GetArtifact(_gameType, "Gibraltar", _version);
            _repository.Received().GetArtifact(_gameType, "888Italy", _version);
        }

        [Test]
        public void Build_ResultedVersionShouldHaveTheCorrect_GameTechnology()
        {
            Init(BuildRegulationArtifact("Gibraltar"));
            
            var gameVersion = _builder.Build();

            Assert.AreEqual((int)_repoDescriptor.Infrastructure.GameTechnology, gameVersion.Technology);
            
        }

        [Test]
        public void Build_ResultedVersionShouldHaveTheCorrect_PlatformType()
        {
            Init(BuildRegulationArtifact("Gibraltar"));
            
            var gameVersion = _builder.Build();

            Assert.AreEqual((int)_repoDescriptor.Infrastructure.PlatformType, gameVersion.PlatformType);
        }


        [Test]
        public void Build_VersionFolderProperty_ShouldBeEqualToRequestedVersion()
        {
            Init(BuildRegulationArtifact("Gibraltar"));
            
            var gameVersion = _builder.Build();

            Assert.AreEqual(_version, gameVersion.VersionFolder);
        }


        [Test]
        public void Build_IfArtifactHasAVersionProperty_VersionAsLongShouldBeEqualToTheValueOfTheProperty()
        {
            Init(BuildRegulationArtifact("Gibraltar", "1.3.5.1"));
            
            var gameVersion = _builder.Build();

            Assert.AreEqual(new VersionNumber("1.3.5.1").ToLong(), gameVersion.VersionAsLong);
        }


        [Test]
        public void Build_IfArtifactDoesntHaveVersionProperty_VersionAsLongShouldBeEqualToVersionFolder()
        {  
            
            Init(BuildRegulationArtifact("Gibraltar"));
            
            var gameVersion = _builder.Build();

            Assert.AreEqual(new VersionNumber(_version).ToLong(), gameVersion.VersionAsLong);
        }


        [Test]
        public void Build_CreatedDateField_EqualsToCreatedDateFromArtifact()
        {
            var regulationArtifact = BuildRegulationArtifact("Gibraltar");
            regulationArtifact.Artifact.created = new DateTime(2015, 5, 10);

            Init(regulationArtifact);
            
            var gameVersion = _builder.Build();

            Assert.AreEqual(new DateTime(2015, 5, 10), gameVersion.CreatedDate);
        }


        [Test]
        public void Build_CreatedByField_EqualsToCreatedByFromArtifact()
        {
            var regulationArtifact = BuildRegulationArtifact("Gibraltar");
            regulationArtifact.Artifact.createdBy = "Florin";

            Init(regulationArtifact);
            
            var gameVersion = _builder.Build();

            Assert.AreEqual("Florin", gameVersion.CreatedBy);
        }


        [Test]
        public void Build_TriggeredByFieldValue_ShouldBeTakenFromTheArtifactoryProperty()
        {
            var regulationArtifact = BuildRegulationArtifact("Gibraltar");

            regulationArtifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty(WellKnownNamesAndValues.UserProperty, "elik")
            };
            
            Init(regulationArtifact);
            
            var gameVersion = _builder.Build();

            Assert.AreEqual("elik", gameVersion.TriggeredBy);
        }

        [Test]
        public void Build_IfMoreThanOneArtifactsWithDifferentVersionProperty_UseTheFolderVersionForTheVersionAsLong()
        {
            
            Init(BuildRegulationArtifact("Gibraltar", "2.0.0.0"),
                 BuildRegulationArtifact("888Italy", "3.0.0.0"));
            
            var gameVersion = _builder.Build();

            Assert.AreEqual(new VersionNumber(_version).ToLong(), gameVersion.VersionAsLong);
        }

        [Test]
        public void Build_IfMoreThanOneArtifactsAllWithTheSameVersionProperty_UseTheVersionPropertyFromOneOfTheArtifacts()
        {
            Init(BuildRegulationArtifact("Gibraltar", "2.0.0.0"),
                 BuildRegulationArtifact("888Italy", "2.0.0.0"));

            
            var gameVersion = _builder.Build();

            Assert.AreEqual(new VersionNumber("2.0.0.0").ToLong(), gameVersion.VersionAsLong);
        }


        [Test]
        public void Build_IfVersionPropertyIsDifferentBetweenArtifacts_WriteMessageToLog()
        {
            string actuallMessage = null;
            _logger.Info(Arg.Do<string>(arg => actuallMessage = arg));

            Init(BuildRegulationArtifact("Gibraltar", "2.0.0.0"),
                 BuildRegulationArtifact("Spain", "3.0.0.0"),
                 BuildRegulationArtifact("Denmark", "2.0.0.0"),
                 BuildRegulationArtifact("888Italy", "3.0.0.0")
                 );

            var gameVersion = _builder.Build();
            

            Assert.IsTrue(actuallMessage.Contains("Version property has different values for different regulations"));
            Assert.IsTrue(actuallMessage.Contains(_repoDescriptor.Repository.GetRootFolderRelativeUrl()));
            Assert.IsTrue(actuallMessage.Contains(_gameType.ToString()));
            Assert.IsTrue(actuallMessage.Contains("2.0.0.0 for Denmark & Gibraltar"));
            Assert.IsTrue(actuallMessage.Contains("3.0.0.0 for 888Italy & Spain"));

        }

        [Test]
        public void Build_IfNoVersionProperty_WriteMessageToLog()
        {
            string actuallMessage = null;
            _logger.Info(Arg.Do<string>(arg => actuallMessage = arg));

            Init(BuildRegulationArtifact("Gibraltar"),
                 BuildRegulationArtifact("Spain"),
                 BuildRegulationArtifact("Denmark"),
                 BuildRegulationArtifact("888Italy")
                 );

            var gameVersion = _builder.Build();


            Assert.IsTrue(actuallMessage.Contains("Missing version property"));
            Assert.IsTrue(actuallMessage.Contains(_repoDescriptor.Repository.GetRootFolderRelativeUrl()));
            Assert.IsTrue(actuallMessage.Contains(_gameType.ToString()));
            Assert.IsTrue(actuallMessage.Contains(_version));
            Assert.IsTrue(actuallMessage.Contains("Gibraltar"));
            Assert.IsTrue(actuallMessage.Contains("Spain"));
            Assert.IsTrue(actuallMessage.Contains("Denmark"));
            Assert.IsTrue(actuallMessage.Contains("888Italy"));
        }


        [Test]
        public void CanBuild_IfNoArtifactIsFound_ReturnFalse()
        {
            Init(new RegulationArtifact[0]);
            
            Assert.AreEqual(false, _builder.CanBuild());
        }



        [Test]
        public void CanBuild_IfNoRegulationIsFound_ReturnFalse()
        {
            Init();
            
            Assert.AreEqual(false, _builder.CanBuild());

        }

        [Test]
        public void Build_TheNumberOfRecordsInTheGameversion_RegulationsShouldBeEqualToTheNumberOfRegulationsOfTheGame()
        {
            Init(BuildRegulationArtifact("Gibraltar", "2.0.0.0"),
                 BuildRegulationArtifact("Spain", "3.0.0.0"));

            InitBuilder();

            var gameVersionRecord = _builder.Build();
            
            Assert.AreEqual(2, gameVersionRecord.GameVersion_Regulations.Count);

        }


        [Test]
        public void Build_TheNumberOfRecordsInTheGameversion_Properties_ShouldBeEqualToTheNumberOfPropertiesFromAllRegulationArtifacts()
        {
            var a1 = BuildRegulationArtifact("Gibraltar", "2.0.0.0");
            var a2 = BuildRegulationArtifact("Spain", "2.0.0.0");

            a1.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty("Prop1", "Value1"),
                new ArtifactoryProperty("Prop2", "Value2"),
            };


            a2.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty("Prop3", "Value2"),
                new ArtifactoryProperty("Prop4", "Value4"),
            };

            Init(a1, a2);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            Assert.AreEqual(4, gameVersionRecord.GameVersion_Properties.Count);

        }

     

        [Test]
        public void Build_IfPropertyIsConfiguredAsIgnored_DontWriteAnyWarnToLog()
        {
            
            var settings = new ArtifactorySettings();
            settings.IgnoreUndefinedPropertiesValues = new IgnoredUndefinedPropertyValueSettingsCollection(
                                                                    new IgnoredUndefinedPropertyValueSettings("NDL.State", "XXX"));

            _internalServices.ConfigurationReader.ReadSection<ArtifactorySettings>().Returns(settings);

            var propertySet = new PropertySet("NDL", new PropertyDefinition("State", PropertyType.SINGLE_SELECT,
                                                                                    new PropertyPredefinedValue("InProgress", true),
                                                                                    new PropertyPredefinedValue("Approved", true)));

            _internalServices.ArtifactorySynchronizationManager.GetAvailablePropertySets().Returns(new PropertySet[] { propertySet });

            var artifact = BuildRegulationArtifact("Gibraltar", "2.0.0.0");
            

            artifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty("NDL.State", "XXX"),
            };
            
            Init(artifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            _logger.DidNotReceive().Warn(Arg.Any<string>());
        }

        [Test]
        public void Build_IfPropertyTypeIsAny_DontWriteAnyWarnToLogToLog()
        {
            var settings = new ArtifactorySettings();
            settings.IgnoreUndefinedPropertiesValues = new IgnoredUndefinedPropertyValueSettingsCollection();

            _internalServices.ConfigurationReader.ReadSection<ArtifactorySettings>().Returns(settings);

            var propertySet = new PropertySet("NDL", new PropertyDefinition("State", PropertyType.ANY_VALUE,
                                                                                    new PropertyPredefinedValue("InProgress", true),
                                                                                    new PropertyPredefinedValue("Approved", true)));

            _internalServices.ArtifactorySynchronizationManager.GetAvailablePropertySets().Returns(new PropertySet[] { propertySet });

            var artifact = BuildRegulationArtifact("Gibraltar", "2.0.0.0");


            artifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty("NDL.State", "XXX"),
            };

            Init(artifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            _logger.DidNotReceive().Warn(Arg.Any<string>());
        }


        [TestCase(PropertyType.SINGLE_SELECT)]
        [TestCase(PropertyType.MULTI_SELECT)]
        public void Build_IfPropertyValueIsNotWhatWeExpect_WriteToLog(PropertyType propertyType)
        {
            string actualMessage = null;
            _logger.Warn(Arg.Do<string>(arg => actualMessage = arg));

            var settings = new ArtifactorySettings();
            settings.IgnoreUndefinedPropertiesValues = new IgnoredUndefinedPropertyValueSettingsCollection();

            _internalServices.ConfigurationReader.ReadSection<ArtifactorySettings>().Returns(settings);

            var propertySet = new PropertySet("NDL", new PropertyDefinition("State", propertyType,
                                                                                    new PropertyPredefinedValue("InProgress", true),
                                                                                    new PropertyPredefinedValue("Approved", true)));

            _internalServices.ArtifactorySynchronizationManager.GetAvailablePropertySets().Returns(new PropertySet[] { propertySet });

            var artifact = BuildRegulationArtifact("Gibraltar", "2.0.0.0");


            artifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty("NDL.State", "XXX"),
            };

            Init(artifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();
            

            Assert.IsTrue(actualMessage.Contains("NDL.State"));
            Assert.IsTrue(actualMessage.Contains("XXX"));
        }


        [Test]
        public void Build_IfTheGameIsQAApproved_SetTheQAApprovalDateToCurrentDate()
        {
            var regulation = Substitute.For<IRegulation>();
            
            _internalServices.RegulationsDictionary.GetRegulation("Spain").Returns(regulation);
            DateTime today = new DateTime(2016, 11, 2);
            _internalServices.TimeServices.Now.Returns(today);

            var regulationArtifact = BuildRegulationArtifact("Spain", "2.0.0.0");
            regulationArtifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty(WellKnownNamesAndValues.QAApproved, WellKnownNamesAndValues.True),
            };

            Init(regulationArtifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            var clientTypeRecord = gameVersionRecord.GameVersion_Regulations[0];
            Assert.AreEqual(today, clientTypeRecord.QAApprovalDate);
            Assert.AreEqual("smaster", clientTypeRecord.QAApprovalUser);

        }


        [Test]
        public void Build_IfTheGameIsNotQAApproved_LeaveQAApprovalDateAndUserNameNUll()
        {
            var regulation = Substitute.For<IRegulation>();
            _internalServices.RegulationsDictionary.GetRegulation("Spain").Returns(regulation);

            var regulationArtifact = BuildRegulationArtifact("Spain", "2.0.0.0");
            regulationArtifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty(WellKnownNamesAndValues.QAApproved, WellKnownNamesAndValues.False),
            };

            Init(regulationArtifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            var regulationRecord = gameVersionRecord.GameVersion_Regulations[0];
            Assert.IsNull(regulationRecord.QAApprovalDate);
            Assert.IsNull(regulationRecord.QAApprovalUser);

        }

        [TestCase("TRUE")]
        [TestCase("true")]
        [TestCase("True")]
        public void Build_IfTheGameIsPMApproved_SetThePMApprovalDateToCurrentDate(string approvalState)
        {
            var regulation = Substitute.For<IRegulation>();
            
            _internalServices.RegulationsDictionary.GetRegulation("Spain").Returns(regulation);
            DateTime today = new DateTime(2016, 11, 2);
            _internalServices.TimeServices.Now.Returns(today);

            var regulationArtifact = BuildRegulationArtifact("Spain", "2.0.0.0");
            regulationArtifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty(WellKnownNamesAndValues.PMApproved, WellKnownNamesAndValues.True),
            };

            Init(regulationArtifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            var clientTypeRecord = gameVersionRecord.GameVersion_Regulations[0];
            Assert.AreEqual(today, clientTypeRecord.PMApprovalDate);
            Assert.AreEqual("smaster", clientTypeRecord.PMApprovalUser);

        }

        [Test]
        public void Build_IfTheGameIsNotPMApproved_LeavePMApprovalDateAndUserNameNUll()
        {
            var regulation = Substitute.For<IRegulation>();
            

            _internalServices.RegulationsDictionary.GetRegulation("Spain").Returns(regulation);

            var regulationArtifact = BuildRegulationArtifact("Spain", "2.0.0.0");
            regulationArtifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty(WellKnownNamesAndValues.PMApproved, WellKnownNamesAndValues.False),
            };

            Init(regulationArtifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            var clientTypeRecord = gameVersionRecord.GameVersion_Regulations[0];
            Assert.IsNull(clientTypeRecord.PMApprovalDate);
            Assert.IsNull(clientTypeRecord.PMApprovalUser);

        }

        [Test]
        public void Build_IfApprovalPropertiesMissingForASpecificClient_LeaveQAandPMApprovalDateAndUserNameNULL()
        {
            var regulation = Substitute.For<IRegulation>();
            

            _internalServices.RegulationsDictionary.GetRegulation("Spain").Returns(regulation);

            var regulationArtifact = BuildRegulationArtifact("Spain", "2.0.0.0");
            regulationArtifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty("Something", "value"),
            };

            Init(regulationArtifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            var clientTypeRecord = gameVersionRecord.GameVersion_Regulations[0];
            Assert.IsNull(clientTypeRecord.QAApprovalDate);
            Assert.IsNull(clientTypeRecord.QAApprovalUser);
            Assert.IsNull(clientTypeRecord.PMApprovalDate);
            Assert.IsNull(clientTypeRecord.PMApprovalUser);

        }

        [Test]
        public void Build_IfTheGameIsInProduction_SetTheProductionApprovalDateToCurrentDate()
        {
            var regulation = Substitute.For<IRegulation>();
            
            _internalServices.RegulationsDictionary.GetRegulation("Spain").Returns(regulation);
            DateTime today = new DateTime(2016, 11, 2);
            _internalServices.TimeServices.Now.Returns(today);

            var regulationArtifact = BuildRegulationArtifact("Spain", "2.0.0.0");
            regulationArtifact.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                new ArtifactoryProperty(WellKnownNamesAndValues.Production, WellKnownNamesAndValues.True),
            };

            Init(regulationArtifact);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            var clientTypeRecord = gameVersionRecord.GameVersion_Regulations[0];
            Assert.AreEqual(today, clientTypeRecord.ProductionUploadDate);
        }
        
        [Test]
        public void Build_ForEachArtifact_ShouldBuildTheGameVersionLanguageRecords()
        {
            var a1 = BuildRegulationArtifact("Gibraltar", "2.0.0.0");
            var a2 = BuildRegulationArtifact("Spain", "2.0.0.0");

            a1.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                LanguageProperty.FromLanguage("en", "hash1")
            };


            a2.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                LanguageProperty.FromLanguage("en", "hash1")
            };

            Init(a1, a2);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            Assert.AreEqual(2, gameVersionRecord.GameVersion_Languages.Count);
        }

        [Test]
        public void Build_IfTheLanguageCodeDoesNotExists_ShouldNotCreateTheLanguageRecord()
        {
            var a1 = BuildRegulationArtifact("Gibraltar", "2.0.0.0");

            a1.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                LanguageProperty.FromLanguage("xx", "hash1")
            };

            Init(a1);

            InitBuilder();

            var gameVersionRecord = _builder.Build();

            Assert.AreEqual(0, gameVersionRecord.GameVersion_Languages.Count);
        }

        [Test]
        public void Build_IfTheLanguageCodeDoesNotExists_ShouldLogTheLanguageAndArtifactUri()
        {
            var a1 = BuildRegulationArtifact("Gibraltar", "2.0.0.0");

            a1.Artifact.Properties = new ArtifactoryPropertyCollection()
            {
                LanguageProperty.FromLanguage("xx", "hash1")
            };

            a1.Artifact.uri = "http://url";
            
            Init(a1);
                     
            string errorMessage = null;
            _logger.Error(Arg.Do<string>(arg => errorMessage = arg));

            InitBuilder();
            
            var gameVersionRecord = _builder.Build();

            Assert.IsTrue(errorMessage.Contains(" xx "));
            Assert.IsTrue(errorMessage.Contains("http://url"));
        }

        [Test]
        public void Build_IfTheLanguageIsApproved_SetTheQAApprovalDateAndQAApprovalUser()
        {
            var qaApprovalDate = new DateTime(2016, 4, 5);
            _internalServices.TimeServices.Now.Returns(qaApprovalDate);

            var a = BuildRegulationArtifact("Gibraltar", "2.0.0.0");
            

            a.Artifact.Properties = new ArtifactoryPropertyCollection
            {
                LanguageProperty.FromLanguage("en", "hash1"),
                LanguageProperty.FromLanguage("es", "hash2"),
                LanguageProperty.BuildLanguageQaApprovedProperty("en,es")
            };
            
            
            Init(a);

            InitBuilder();

            var gameVersinoRecord = _builder.Build();

            Assert.IsTrue(gameVersinoRecord.GameVersion_Languages.All(languageRecord => languageRecord.QAApprovalDate == qaApprovalDate));
            Assert.IsTrue(gameVersinoRecord.GameVersion_Languages.All(languageRecord => languageRecord.QAApprovalUser == "smaster"));
        }


        [Test]
        public void Build_IfTheLanguageIsInProduction_SetTheProductionUploadDate()
        {
            var productionUploadDate = new DateTime(2016, 4, 5);
            _internalServices.TimeServices.Now.Returns(productionUploadDate);

            var a = BuildRegulationArtifact("Gibraltar", "2.0.0.0");


            a.Artifact.Properties = new ArtifactoryPropertyCollection
            {
                LanguageProperty.FromLanguage("en", "hash1"),
                LanguageProperty.FromLanguage("es", "hash2"),
                LanguageProperty.BuildLanguageProductionProperty("en,es")
            };


            Init(a);

            InitBuilder();

            var gameVersinoRecord = _builder.Build();

            Assert.IsTrue(gameVersinoRecord.GameVersion_Languages.All(languageRecord => languageRecord.ProductionUploadDate == productionUploadDate));
            

        }
    }



}
