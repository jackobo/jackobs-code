using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using Spark.Infra.Configurations;
using GamesPortal.Service.Entities;

namespace GamesPortal.Service.Artifactory
{
    [ConfigurationSection("artifactory")]
    public class ArtifactorySettings : ConfigurationSectionBase
    {
        [System.Configuration.ConfigurationProperty("baseUrl", IsRequired=true)]
        public string BaseUrl
        {
            get
            {
                return (string)this["baseUrl"];
            }
            set
            {
                this["baseUrl"] = value;
            }
        }

        [ConfigurationProperty("userName", DefaultValue = "smaster")]
        public string UserName
        {
            get
            {
                return (string)this["userName"];
            }
            set
            {
                this["userName"] = value;
            }
        }

        [ConfigurationProperty("password", DefaultValue = "MasterS1")]
        public string Password
        {
            get
            {
                return (string)this["password"];
            }
            set
            {
                this["password"] = value;
            }
        }

        [System.Configuration.ConfigurationProperty("synchronizationEnabled", DefaultValue=true)]
        public bool SynchronizationEnabled
        {
            get
            {
                return (bool)this["synchronizationEnabled"];
            }
            set
            {
                this["synchronizationEnabled"] = value;
            }
        }


        [System.Configuration.ConfigurationProperty("synchronizationInterval", DefaultValue = "01:00:00")]
        public TimeSpan SynchronizationInterval
        {
            get
            {
                return (TimeSpan)this["synchronizationInterval"];
            }
            set
            {
                this["synchronizationInterval"] = value;
            }
        }

        [System.Configuration.ConfigurationProperty("ignoreUndefinedPropertiesValues")]
        public IgnoredUndefinedPropertyValueSettingsCollection IgnoreUndefinedPropertiesValues
        {
            get { return (IgnoredUndefinedPropertyValueSettingsCollection)this["ignoreUndefinedPropertiesValues"]; }
            set { this["ignoreUndefinedPropertiesValues"] = value; }
        }

        [ConfigurationProperty("enableGamingComponentsSynchronization", DefaultValue=false)]
        public bool EnableGamingComponentsSynchronization
        {
            get
            {
                return (bool)this["enableGamingComponentsSynchronization"];
            }
            set
            {
                this["enableGamingComponentsSynchronization"] = value;
            }
        }

        public IEnumerable<IArtifactoryRepositoryDescriptor> GetAllComponentsRepositoryDescriptors(IArtifactoryRestClientFactory artifactoryRestClientFactory)
        {
            var result = new List<IArtifactoryRepositoryDescriptor>();
            result.AddRange(this.ChillWrapperRepositories.GetChillWrapperRespositories(artifactoryRestClientFactory));
            result.AddRange(this.GamesRepositories.GetGamesRespositories(artifactoryRestClientFactory));
            return result;
        }

        [ConfigurationProperty("gamesRepositories")]
        public GamesRepositorySettingsCollection GamesRepositories
        {
            get { return (GamesRepositorySettingsCollection)this["gamesRepositories"]; }
            set { this["gamesRepositories"] = value; }
        }


        [ConfigurationProperty("chillWrapperRepositories")]
        public ChillWrapperRepositorySettingsCollection ChillWrapperRepositories
        {
            get { return (ChillWrapperRepositorySettingsCollection)this["chillWrapperRepositories"]; }
            set { this["chillWrapperRepositories"] = value; }
        }

    }


    public class IgnoredUndefinedPropertyValueSettings : ConfigurationElement
    {
        public IgnoredUndefinedPropertyValueSettings()
        {

        }

        public IgnoredUndefinedPropertyValueSettings(string key, string value)
        {
            Key = key;
            Value = value;
        }
        [ConfigurationProperty("key", IsRequired=true)]
        public string Key
        {
            get { return (string)this["key"]; }
            set { this["key"] = value; }
        }

        [ConfigurationProperty("value", IsRequired = true)]
        public string Value
        {
            get { return (string)this["value"]; }
            set { this["value"] = value; }
        }
    }


    public class IgnoredUndefinedPropertyValueSettingsCollection : ConfigurationCollectionBase<IgnoredUndefinedPropertyValueSettings>
    {
        public IgnoredUndefinedPropertyValueSettingsCollection()
        {

        }

        public IgnoredUndefinedPropertyValueSettingsCollection(params IgnoredUndefinedPropertyValueSettings[] ignoredPropertiesValues)
        {
            foreach (var p in ignoredPropertiesValues)
                BaseAdd(p);
        }
        protected override object GetItemKey(IgnoredUndefinedPropertyValueSettings item)
        {
            return item.Key + "=" + item.Value;
        }

    }

    public class GamesRepositorySettingsCollection : ConfigurationCollectionBase<GamesRepositorySettings>
    {
        protected override object GetItemKey(GamesRepositorySettings item)
        {
            return (item.Name + "/" + item.GamesFolder).ToLowerInvariant();
        }

        public IEnumerable<GamesRepositoryDescriptor> GetGamesRespositories(IArtifactoryRestClientFactory restClientFactory)
        {
            var repositories = new List<GamesRepositoryDescriptor>();

            foreach(GamesRepositorySettings repoSettings in this)
            {
                repositories.Add(repoSettings.CreateRepositoryDescriptor(restClientFactory));
            }

            return repositories;
        }


        protected override void InitializeDefault()
        {
            base.InitializeDefault();

            this.AddElementName = "repository";
        }


        protected override void Init()
        {
            base.Init();
            this.AddElementName = "repository";
        }
    }

    public class GamesRepositorySettings : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("gamesFolder", IsRequired = true)]
        public string GamesFolder
        {
            get { return (string)this["gamesFolder"]; }
            set { this["gamesFolder"] = value; }
        }

        [ConfigurationProperty("gameTechnology", IsRequired =true)]
        public GameTechnology? GameTechnology
        {
            get
            {
                return (GameTechnology?)this["gameTechnology"];
            }
            set
            {
                this["gameTechnology"] = value;
            }
        }

        [ConfigurationProperty("platformType", IsRequired = true)]
        public PlatformType? PlatformType
        {
            get
            {
                return (PlatformType?)this["platformType"];
            }
            set
            {
                this["platformType"] = value;
            }
        }

        [ConfigurationProperty("isExternal", IsRequired = true)]
        public bool? IsExternal
        {
            get { return (bool?)this["isExternal"]; }
            set { this["isExternal"] = value; }
        }

        public GamesRepositoryDescriptor CreateRepositoryDescriptor(IArtifactoryRestClientFactory restClientFactory)
        {
            return new GamesRepositoryDescriptor(this.GameTechnology.Value, 
                                                this.IsExternal.Value, 
                                                this.PlatformType.Value, 
                                                new GamesRepository(this.Name, this.GamesFolder, restClientFactory));
        }
    }

    public class ChillWrapperRepositorySettingsCollection : ConfigurationCollectionBase<ChillWrapperRepositorySettings>
    {
        public ChillWrapperRepositorySettingsCollection()
        {

        }

        protected override object GetItemKey(ChillWrapperRepositorySettings item)
        {
            return item.Name + "/" + item.Folder;
        }

        public IEnumerable<ChillWrapperRepositoryDescriptor> GetChillWrapperRespositories(IArtifactoryRestClientFactory restClientFactory)
        {
            var repositories = new List<ChillWrapperRepositoryDescriptor>();

            foreach (ChillWrapperRepositorySettings repoSettings in this)
            {
                repositories.Add(repoSettings.CreateRepositoryDescriptor(restClientFactory));
            }

            return repositories;
        }


        protected override void InitializeDefault()
        {
            base.InitializeDefault();

            this.AddElementName = "repository";
        }

        protected override void Init()
        {
            base.Init();
            this.AddElementName = "repository";
        }
    }

    public class ChillWrapperRepositorySettings : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public string Name
        {
            get { return (string)this["name"]; }
            set { this["name"] = value; }
        }

        [ConfigurationProperty("folder", IsRequired = true)]
        public string Folder
        {
            get { return (string)this["folder"]; }
            set { this["folder"] = value; }
        }

        [ConfigurationProperty("category", IsRequired = true)]
        public GamingComponentCategory? Category
        {
            get
            {
                return (GamingComponentCategory?)this["category"];
            }
            set
            {
                this["category"] = value;
            }
        }
             
        public ChillWrapperRepositoryDescriptor CreateRepositoryDescriptor(IArtifactoryRestClientFactory restClientFactory)
        {
            var componentTypeOptional = ChillWrapperType.TryFindFromId(this.Category.Value);

            foreach(var componentType in componentTypeOptional)
            {
              
                return new ChillWrapperRepositoryDescriptor(componentType.Infrastructure,
                                                            new ChillWrapperRepository(this.Name, this.Folder, restClientFactory, componentType.Id));
                
            }


            throw new ApplicationException("Invalid component type " + this.Category);
        }
    }
}
