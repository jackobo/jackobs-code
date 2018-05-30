using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.DataAccessLayer;
using Spark.Infra.Types;

namespace GamesPortal.Service.Artifactory
{

    public interface IRepositoryHolder
    {
        IComponentTypeHolder FromRepository(IArtifactoryRepositoryDescriptor repository);
    }

    public interface IComponentTypeHolder
    {
        IComponentIdHolder WithComponentType(int componentType);
    }


    public interface IComponentIdHolder
    {
        IVersionHolder WithComponentId(Guid componentId);
    }


    public interface IVersionHolder
    {
        IComponentVersionBuilder WithVersionFolder(string versionFolder);
    }



    public interface IComponentVersionBuilder
    {
        bool CanBuild();
        GameVersion Build();
    }

    public class ComponentVersionRecordBuilder
                            : IRepositoryHolder
                             , IComponentTypeHolder
                             , IComponentIdHolder
                             , IVersionHolder
                             , IComponentVersionBuilder

       
       

    {

        protected ComponentVersionRecordBuilder(IArtifactorySyncronizationLogger logger, IGamesPortalInternalServices services)
        {
            Logger = logger;
            Services = services;
            FillAvailablePropertySets();
            FillIgnoredUndefinedProperties();
        }

        Dictionary<string, PropertySet> _artifactoryPropertySets = new Dictionary<string, PropertySet>();
        Dictionary<string, List<string>> _ignoredUndefinedPropertiesValues = new Dictionary<string, List<string>>();

        private void FillAvailablePropertySets()
        {
            _artifactoryPropertySets = (Services.ArtifactorySynchronizationManager.GetAvailablePropertySets() ?? new PropertySet[0]).ToDictionary(ps => ps.name);
        }


        private void FillIgnoredUndefinedProperties()
        {
            var ignoredPropertiesValues = Services.ConfigurationReader.ReadSection<ArtifactorySettings>()
                                                ?.IgnoreUndefinedPropertiesValues
                                                ?? new IgnoredUndefinedPropertyValueSettingsCollection();

            foreach (IgnoredUndefinedPropertyValueSettings ingnoredPropertiesValue in ignoredPropertiesValues)
            {
                if (!_ignoredUndefinedPropertiesValues.ContainsKey(ingnoredPropertiesValue.Key))
                    _ignoredUndefinedPropertiesValues.Add(ingnoredPropertiesValue.Key, new List<string>());

                _ignoredUndefinedPropertiesValues[ingnoredPropertiesValue.Key].Add(ingnoredPropertiesValue.Value);
            }
        }

        protected IGamesPortalInternalServices Services { get; private set; }
        private string GetComponentTypeForLogging(int componentId)
        {
            return "ComponentTypeID = " + componentId;
        }

        protected IArtifactorySyncronizationLogger Logger { get; private set; }
        List<RegulationArtifact> _regulationArtifacts;

        #region Builder Protocol

        IArtifactoryRepositoryDescriptor _repositoryDescriptor;
        IComponentTypeHolder IRepositoryHolder.FromRepository(IArtifactoryRepositoryDescriptor repository)
        {
            _repositoryDescriptor = repository;
            return this;

        }



        int _componentType;
        string[] _regulations;
        IComponentIdHolder IComponentTypeHolder.WithComponentType(int componentType)
        {
            _componentType = componentType;
            _regulations = _repositoryDescriptor.Repository.GetComponentRegulations(componentType);

            return this;
        }

        public static IRepositoryHolder Init(IArtifactorySyncronizationLogger logger, IGamesPortalInternalServices services)
        {
            return new ComponentVersionRecordBuilder(logger, services);
        }

        Guid _componentId;

        IVersionHolder IComponentIdHolder.WithComponentId(Guid componentId)
        {
            _componentId = componentId;
            return this;
        }


        string _versionFolder;

        IComponentVersionBuilder IVersionHolder.WithVersionFolder(string versionFolder)
        {
            _versionFolder = versionFolder;

            _regulationArtifacts = new List<RegulationArtifact>();

            foreach (var regulation in _regulations)
            {
                _repositoryDescriptor.Repository.GetArtifact(_componentType, regulation, _versionFolder)
                                                .Do(artifact => _regulationArtifacts.Add(new RegulationArtifact(regulation, artifact)));
            }

           


            return this;
        }


        GameVersion IComponentVersionBuilder.Build()
        {
            return this.Build();
        }


        bool IComponentVersionBuilder.CanBuild()
        {
            return _regulationArtifacts.Count > 0;
        }
        #endregion

        protected virtual GameVersion Build()
        {

        

            var componentVersion = BuildComponentVersionRecord(_regulationArtifacts);

            componentVersion.GameVersion_Regulations.AddRange(BuildRegulationRecords(componentVersion.GameVersion_ID));
            componentVersion.GameVersion_Properties.AddRange(BuildComponentVersionPropertyRecords(componentVersion.GameVersion_ID));


            return componentVersion;
        }

        protected virtual IEnumerable<GameVersion_Property> BuildComponentVersionPropertyRecords(Guid componentVersion_ID)
        {
            var records = new List<GameVersion_Property>();

            foreach (var regulationArtifact in _regulationArtifacts)
            {
                foreach (var property in regulationArtifact.Artifact.Properties)
                {
                    CheckPropertyValues(property, regulationArtifact.Regulation);
                    records.Add(BuildComponentVersionProperty(componentVersion_ID, regulationArtifact, property));
                }

            }

            return records;
        }

        private ArtifactoryProperty GetUndefinedPropertyValues(ArtifactoryProperty property)
        {

            if (!property.HasPropertySet
                || !_artifactoryPropertySets.ContainsKey(property.SetName))
            {
                return new ArtifactoryProperty(property.Key);
            }

            
            var propertyDefinition = _artifactoryPropertySets[property.SetName]
                                                    .properties
                                                    .FirstOrDefault(p => p.name == property.Name);
            

            if (propertyDefinition == null || propertyDefinition.propertyType == PropertyType.ANY_VALUE)
                return new ArtifactoryProperty(property.Key);

            var ignoredProperties = _ignoredUndefinedPropertiesValues.ContainsKey(property.Key)
                                                                     ? _ignoredUndefinedPropertiesValues[property.Key]
                                                                     : new List<string>();

            var undefinedValues = property.Values.Where(v => 
                                                        !propertyDefinition.predefinedValues.Select(pv => pv.value).Contains(v));
            

            undefinedValues = undefinedValues.Where(v => !ignoredProperties.Contains(v));


            return new ArtifactoryProperty(property.Key, undefinedValues.ToArray());
        }
        

        private void CheckPropertyValues(ArtifactoryProperty property, string regulation)
        {

            var propertyWithUndefinedValues = GetUndefinedPropertyValues(property);

            
            
            if (propertyWithUndefinedValues.Values.Length > 0)
            {
                Logger.Warn(string.Format("Undefined values ({0}) detected for property {1}! Repository = {2}; {3}; Regulation = {4}; VersionFolder = {5}",
                                                 string.Join(", ", propertyWithUndefinedValues.Values),
                                                 property.Key,
                                                 _repositoryDescriptor.Repository.GetRootFolderRelativeUrl(),
                                                 GetComponentTypeForLogging(_componentType),
                                                 regulation,
                                                 _versionFolder));

            }

        }

        protected virtual GameVersion_Property BuildComponentVersionProperty(Guid componentVersion_ID, RegulationArtifact regulationArtifact, ArtifactoryProperty property)
        {
            var propertyRecord = new GameVersion_Property();
            propertyRecord.GameVersion_ID = componentVersion_ID;
            propertyRecord.PropertyKey = property.Key;
            propertyRecord.PropertyName = property.Name;
            propertyRecord.PropertySet = property.SetName;
            propertyRecord.PropertyValue = property.ConcatValues();
            propertyRecord.Regulation = regulationArtifact.Regulation;
            propertyRecord.Row_ID = Guid.NewGuid();
            return propertyRecord;
        }

        protected virtual GameVersion BuildComponentVersionRecord(IEnumerable<RegulationArtifact> regulationArtifacts)
        {
            var componentVersionRecord = new GameVersion()
            {
                Game_ID = _componentId,
                GameVersion_ID = Guid.NewGuid(),
                Technology = (int)_repositoryDescriptor.Infrastructure.GameTechnology,
                PlatformType = (int)_repositoryDescriptor.Infrastructure.PlatformType,
                VersionFolder = _versionFolder,
                VersionAsLong = BuildVersionAsLong(),
                CreatedBy = BuildCreatedBy(),
                CreatedDate = BuildCreatedDate(),
                TriggeredBy = BuildTriggeredBy()
            };

            componentVersionRecord.GameVersion_Languages.AddRange(BuildLanguageRecords(componentVersionRecord.GameVersion_ID, regulationArtifacts));

            return componentVersionRecord;
        }


        protected virtual IEnumerable<GameVersion_Language> BuildLanguageRecords(Guid gameVersionId, IEnumerable<RegulationArtifact> regulationArtifacts)
        {
            var languageRecords = new List<GameVersion_Language>();
            var currentDateAndTime = Services.TimeServices.Now;

            foreach (var regulationArtifact in regulationArtifacts)
            {
                string[] qaApprovedLanguages = regulationArtifact.Artifact.GetDistinctPropertyValues(LanguageProperty.Language_QAApproved);
                string[] productionLanguages = regulationArtifact.Artifact.GetDistinctPropertyValues(LanguageProperty.Language_Production);

                foreach (var property in regulationArtifact.Artifact.Properties)
                {
                    if (!LanguageProperty.IsLanguageHash(property.Key))
                        continue;

                    var languageHashProperty = LanguageProperty.FromKey(property.Key, property.ConcatValues());
                    var language = Services.LanguageDictionary.TryFindLanguage(languageHashProperty.Language);
                    if (!language.Any())
                    {
                        Logger.Info($"Cannot find language {languageHashProperty.Language} for artifact {regulationArtifact.Artifact.uri}");
                        continue;
                    }

                    var languageRecord = new GameVersion_Language()
                    {
                        GameVersionLanguage_ID = Guid.NewGuid(),
                        GameVersion_ID = gameVersionId,
                        Regulation = regulationArtifact.Regulation,
                        Language = Services.LanguageDictionary.FindLanguage(languageHashProperty.Language).Name,
                        LanguageHash = languageHashProperty.ConcatValues(),
                        ArtifactoryLanguage = languageHashProperty.Language,
                    };

                    if (qaApprovedLanguages.Contains(languageHashProperty.Language))
                    {
                        languageRecord.QAApprovalDate = currentDateAndTime;
                        languageRecord.QAApprovalUser = "smaster";
                    }

                    if (productionLanguages.Contains(languageHashProperty.Language))
                    {
                        languageRecord.ProductionUploadDate = currentDateAndTime;
                    }

                    languageRecords.Add(languageRecord);

                }
            }

            return languageRecords;
        }

        protected virtual IEnumerable<GameVersion_Regulation> BuildRegulationRecords(Guid componentVersionId)
        {
            var regulations = new List<GameVersion_Regulation>();
            foreach(var regulationArtifact in _regulationArtifacts)
            {
                GameVersion_Regulation regulationRecord = BuildComponentVersionRegulationRecord(componentVersionId, regulationArtifact);
                regulations.Add(regulationRecord);

            }

            return regulations;
        }

        protected virtual GameVersion_Regulation BuildComponentVersionRegulationRecord(Guid componentVersionId, RegulationArtifact regulationArtifact)
        {
            //Logger.Warn("There is one artifact that has no download URI! Check GameVersion_Regulation table in the database!");

            var regulationRecord = new GameVersion_Regulation();
            regulationRecord.GameVersion_ID = componentVersionId;
            regulationRecord.DownloadUri = regulationArtifact.Artifact.downloadUri;
            regulationRecord.FileName = regulationArtifact.Artifact.ExtractFileName();
            regulationRecord.FileSize = regulationArtifact.Artifact.size;
            regulationRecord.MD5 = regulationArtifact.Artifact.checksums.md5;
            regulationRecord.SHA1 = regulationArtifact.Artifact.checksums.sha1;
            regulationRecord.Regulation = regulationArtifact.Regulation;
            regulationRecord.GameVersionRegulation_ID = Guid.NewGuid();

            if(regulationArtifact.IsQAApproved)
            {
                regulationRecord.QAApprovalDate = Services.TimeServices.Now;
                regulationRecord.QAApprovalUser = "smaster";
            }

            if(regulationArtifact.IsPMApproved)
            {
                regulationRecord.PMApprovalDate = Services.TimeServices.Now;
                regulationRecord.PMApprovalUser = "smaster";
            }

            if (regulationArtifact.IsInProduction)
            {
                regulationRecord.ProductionUploadDate = Services.TimeServices.Now;
            }



            return regulationRecord;
        }
        
        private string BuildTriggeredBy()
        {
            return _regulationArtifacts?.Select(item => item.Artifact.TriggeredBy)
                              .Where(user => !string.IsNullOrEmpty(user))
                              .FirstOrDefault();
        }

        private string BuildCreatedBy()
        {
            if(_regulationArtifacts.Any())
            {
                return _regulationArtifacts.First().Artifact.createdBy;
            }

            return null;
        }


        private DateTime BuildCreatedDate()
        {
            if (_regulationArtifacts.Any())
            {
                return _regulationArtifacts.First().Artifact.created;
            }

            return default(DateTime);
        }

        private long BuildVersionAsLong()
        {

            var artifactsWithVersionProperty = _regulationArtifacts.Select(item => new { Version = item.Artifact.ExtractVersionFromVersionProperty(), Artifact = item })
                                                .Where(a => a.Version != null)
                                                .GroupBy(a => a.Version)
                                                .Select(group => new { Version = group.Key, Artifacts = group.Select(item => item.Artifact).ToArray() })
                                                .ToArray();

            
            if (artifactsWithVersionProperty.Length == 1)
            {
                return artifactsWithVersionProperty.First().Version.ToLong();
            }
            else if(artifactsWithVersionProperty.Length == 0)
            {
                Logger.Info(string.Format("Missing version property! Repository = {0}; {1}; Regulation = {2}; VersionFolder = {3}",
                                                _repositoryDescriptor.Repository.GetRootFolderRelativeUrl(),
                                                GetComponentTypeForLogging(_componentType),
                                                string.Join(", ", _regulationArtifacts.Select(a => a.Regulation)),
                                                _versionFolder
                                                ));
            }
            else
            {
                Logger.Info(string.Format("Version property has different values for different regulations! Repository = {0}; {1}; VersionFolder = {2}; {3}",
                                               _repositoryDescriptor.Repository.GetRootFolderRelativeUrl(),
                                               GetComponentTypeForLogging(_componentType),
                                               _versionFolder,
                                               string.Join("; ", artifactsWithVersionProperty.Select(item =>
                                               {
                                                   var regulations = string.Join(" & ", item.Artifacts.Select(a => a.Regulation)
                                                                                                      .OrderBy(r => r)
                                                                                                      .ToArray());
                                                   return string.Format("{0} for {1}", item.Version, regulations);
                                                }))));
            }


            return new VersionNumber(_versionFolder).ToLong();

            
        }
        
        
    }


}
