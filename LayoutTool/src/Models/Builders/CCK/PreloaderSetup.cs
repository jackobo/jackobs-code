using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using LayoutTool.Interfaces;
using Newtonsoft.Json.Linq;

namespace LayoutTool.Models.Builders.CCK
{
    public interface IPreloaderSetup
    {
        IEnumerable<IJsonConfigurationFileDescriptor> GetFileDescriptors(BrandEntity brand, SkinEntity skin);
        EnvironmentConnection[] GetEnvironmentConnections(BrandEntity brand, SkinEntity skin);
        string GetNavigationPlanPath(BrandEntity brand, SkinEntity skin);
        IClientLaunchConfigurationBuilder GetLaunchConfigurationBuilder(SkinCode skinCode);
    }


    public interface IClientLaunchConfigurationBuilder
    {
        string Build(ABTestCase abTest, EnvironmentConnection environmentConnection);
    }

    public class ClientLaunchConfigurationBuilder : IClientLaunchConfigurationBuilder
    {
        public ClientLaunchConfigurationBuilder(SkinCode skinCode, string theme, string defaultSkin, string languageId)
        {
            _skinCode = skinCode;
            this.theme = theme;
            this.defaultSkin = defaultSkin;
            this.languageId = languageId;
            this.brand = skinCode.BrandId.ToString();
            this.skin = skinCode.SkinId.ToString();

        }

        SkinCode _skinCode;
        string theme { get; set; }      
        string defaultSkin { get; set; }
        string languageId { get; set; }
        string skin { get; set; }
        string brand { get; set; }
        string lang
        {
            get { return languageId; }
        }

        public string Build(ABTestCase abTest, EnvironmentConnection environmentConnection)
        {
            
            JObject jObject = new JObject();
            jObject.Add(new JProperty(nameof(theme), JValue.CreateString(theme)));
#warning defaultSkin and languageId are obsolete since 7.42
            jObject.Add(new JProperty(nameof(defaultSkin), JValue.CreateString(defaultSkin)));
            jObject.Add(new JProperty(nameof(languageId), JValue.CreateString(languageId)));

            jObject.Add(new JProperty(nameof(brand), JValue.CreateString(brand)));
            jObject.Add(new JProperty(nameof(skin), JValue.CreateString(skin)));
            jObject.Add(new JProperty(nameof(lang), JValue.CreateString(lang)));



            foreach (var configurationFile in _configurationFiles)
            {
                jObject.Add(CreateConfigurationFileProperty(configurationFile.Key, ApplyAbTestCase(abTest, configurationFile.Value)));
            }

            jObject.Add(CreateConfigurationFileProperty("environmentPath", ApplyAbTestCase(abTest, environmentConnection.ConfigurationFilePath)));
            
            return HttpUtility.UrlEncode(jObject.ToString());
        }

        private PathDescriptor ApplyAbTestCase(ABTestCase abTest, PathDescriptor path)
        {
            return abTest?.GetOverrideFileOrNull(path, _skinCode)
                   ?? path;
        }

        private JProperty CreateConfigurationFileProperty(string name, PathDescriptor path)
        {
            return new JProperty(name, JValue.CreateString("../../" + path.ToHttpUrlFormat()));
        }


        private Dictionary<string, PathDescriptor> _configurationFiles = new Dictionary<string, PathDescriptor>();
        

        public void AddConfigurationFile(string configurationName, PathDescriptor filePath)
        {
            _configurationFiles.Add(configurationName, filePath);
        }
        
        
    }



    public class PreloaderSetup : IPreloaderSetup
    {

        public PreloaderSetup()
        {
            ThemeEntries = new List<ThemeEntry>();
            ThemesPaths = new List<ThemePath>();
            EnvironmentsSets = new List<ClientEnvironmentSet>();
        }

        public IEnumerable<IJsonConfigurationFileDescriptor> GetFileDescriptors(BrandEntity brand, SkinEntity skin)
        {
            var filesDescriptors = new List<IJsonConfigurationFileDescriptor>();
            
            var themeEntry = FindThemeEntry(brand, skin);

            filesDescriptors.Add(new FilesDescriptors.NavigationPlanJsonFileDescriptor(new PathDescriptor(themeEntry.ClientProfile.NavigationPlanPath)));
            filesDescriptors.Add(new FilesDescriptors.LanguageJsonFileDescriptor(new PathDescriptor(themeEntry.ClientProfile.LanguagePath)));

            return filesDescriptors;
        }


        private ThemeEntry FindThemeEntry(BrandEntity brand, SkinEntity skin)
        {
            return FindThemeEntry(new SkinCode(brand.Id, skin.Id));
        }
        private ThemeEntry FindThemeEntry(SkinCode skinCode)
        {
            var themeEntries = this.ThemeEntries.Where(te => te.Brand == skinCode.BrandId.ToString() && te.Skin == skinCode.SkinId.ToString());

            if(!themeEntries.Any())
            {
                throw new ArgumentException($"Can't find a Theme Entry in the preloader_setup.json for Brand {skinCode.BrandId} and Skin {skinCode.SkinId}");
            }

            var themeEntry = themeEntries.FirstOrDefault(te => te.Language.ToLowerInvariant().Contains("en"));

            if (themeEntry != null)
                return themeEntry;

            return ThemeEntries.First();
        }

        public EnvironmentConnection[] GetEnvironmentConnections(BrandEntity brand, SkinEntity skin)
        {
            var themeEntry = FindThemeEntry(brand, skin);
            var environmentSet = this.EnvironmentsSets.FirstOrDefault(es => string.Compare(es.Name, themeEntry.ClientProfile.Environmentset, true) == 0);

            if(environmentSet == null)
            {
                throw new ArgumentException($"Couldn't find an environment set for brand {brand.Id}-{brand.Name} and skin {skin.Id}-{skin.Name}");
            }

            return environmentSet.Environmnets.Select(env => new EnvironmentConnection(env.Name, new PathDescriptor(env.Path)))
                                       .ToArray();

        }

        public string GetNavigationPlanPath(BrandEntity brand, SkinEntity skin)
        {
            return FindThemeEntry(brand, skin).ClientProfile.NavigationPlanPath;
        }

        public IClientLaunchConfigurationBuilder GetLaunchConfigurationBuilder(SkinCode skinCode)
        {
            var themeEntry = FindThemeEntry(skinCode);
            var themePaths = FindThemePaths(themeEntry.ClientProfile.Theme);

            var config = new ClientLaunchConfigurationBuilder(skinCode,
                                                       themeEntry.ClientProfile.Theme, 
                                                       themeEntry.ClientProfile.DefaultSkin,
                                                       themeEntry.ClientProfile.LanguageId);

            config.AddConfigurationFile("navigationPlanPath", new PathDescriptor(themeEntry.ClientProfile.NavigationPlanPath));
            config.AddConfigurationFile("zippedResourcesPath", new PathDescriptor(themeEntry.ClientProfile.ZippedResourcesPath));
            config.AddConfigurationFile("iconResources", new PathDescriptor(themeEntry.ClientProfile.IconResources));
            config.AddConfigurationFile("languagePath", new PathDescriptor(themeEntry.ClientProfile.LanguagePath));
            config.AddConfigurationFile("themePath", new PathDescriptor(themePaths.ConfigPath));
            config.AddConfigurationFile("lobbyThemePath_JS", new PathDescriptor(themePaths.LobbyThemePath_JS));
            config.AddConfigurationFile("lobbyThemePath", new PathDescriptor(themePaths.LobbyThemePath));
            config.AddConfigurationFile("preloaderPath_JS", new PathDescriptor(themePaths.PreloaderPath_JS));
            config.AddConfigurationFile("preloaderPath", new PathDescriptor(themePaths.PreloaderPath));
            config.AddConfigurationFile("preloaderLogoPath", new PathDescriptor(themePaths.PreloaderLogoPath));


            return config;
        }


        private ThemePath FindThemePaths(string themeId)
        {
            var themePath = this.ThemesPaths.FirstOrDefault(t => t.Theme == themeId);
            if (themePath == null)
                throw new ArgumentException($"Can't find theme paths for themeId {themeId}");

            return themePath;
        }

        public string DefaultEnvironment { get; set; }

        public List<ThemeEntry> ThemeEntries { get; set; }

        public List<ThemePath> ThemesPaths { get; set; }
        
        public List<ClientEnvironmentSet> EnvironmentsSets { get; set; }

        public class ThemeEntry
        {
            public ThemeEntry()
            {
            }

            public string Brand { get; set; }
            public string Skin { get; set; }
            public string Language { get; set; }

            public ClientProfile ClientProfile { get; set; }
        }
        
        public class ClientProfile
        {
            public string NavigationPlanPath { get; set; }
            public string ZippedResourcesPath { get; set; }

            public string IconResources { get; set; }
            public string LanguagePath { get; set; }
            public string Theme { get; set; }
            public string Environmentset { get; set; }
            public string SimplePreloader { get; set; }
            public string DefaultSkin { get; set; }
            public string LanguageId { get; set; }
        }
        
        public class ThemePath
        {
            

            public string Theme { get; set; }
            public string ConfigPath { get; set; }
            public string PreloaderPath { get; set; }
            public string LobbyThemePath { get; set; }
            public string PreloaderLogoPath { get; set; }

            public string LobbyThemePath_JS { get; set; }
            public string PreloaderPath_JS { get; set; }
        }

        public class ClientEnvironmentSet
        {
            public ClientEnvironmentSet()
            {
                this.Environmnets = new List<ClientEnvironment>();
            }
            public string Name { get; set; }
            public List<ClientEnvironment> Environmnets { get; set; }
        }

        public class ClientEnvironment
        {
            public string Name { get; set; }
            public string Path { get; set; }
        }

        
    }
}
