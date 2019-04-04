using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static LayoutTool.Models.Builders.CCK.DynamicsUtils;

namespace LayoutTool.Models.Builders.CCK
{
    public interface IPreloaderSetupParser
    {
        IPreloaderSetup Parse(string preloaderSetupContent);
    }
    public class PreloaderSetupParser : IPreloaderSetupParser
    {
        public PreloaderSetupParser()
        {
        }
        
        public IPreloaderSetup Parse(string preloaderSetupContent)
        {
            var preloaderSetupJson = JsonConvert.DeserializeObject<dynamic>(preloaderSetupContent);

            var preloaderSetup = new PreloaderSetup();

            preloaderSetup.DefaultEnvironment = ConvertDynamicValue<string>(preloaderSetupJson.defaultEnvironment);

            FillThemeEntries(preloaderSetupJson, preloaderSetup);
            FillThemePaths(preloaderSetupJson, preloaderSetup);
            FillEnvironmentSets(preloaderSetupJson, preloaderSetup);

            return preloaderSetup;
        }

        private void FillEnvironmentSets(dynamic preloaderSetupJson, PreloaderSetup preloaderSetup)
        {
            JObject environmnetSetsJson = preloaderSetupJson.environmentSets;

            foreach(JProperty environmentSetJson in environmnetSetsJson.Properties())
            {
                preloaderSetup.EnvironmentsSets.Add(CreateEnvironmentSet(environmentSetJson));
            }

        }

        private PreloaderSetup.ClientEnvironmentSet CreateEnvironmentSet(JProperty environmentSetJson)
        {
            return new PreloaderSetup.ClientEnvironmentSet()
            {
                Name = environmentSetJson.Name,
                Environmnets = CreateEnvironments((JArray)environmentSetJson.Value)
            };
        }

        private List<PreloaderSetup.ClientEnvironment> CreateEnvironments(JArray environmnetsJsonArray)
        {
            var environments = new List<PreloaderSetup.ClientEnvironment>();

            foreach(dynamic environmentJson in  environmnetsJsonArray)
            {
                environments.Add(new PreloaderSetup.ClientEnvironment()
                {
                    Name = ConvertDynamicValue<string>(environmentJson.environment),
                    Path = ConvertDynamicValue<string>(environmentJson.path)
                });
            }


            return environments;
        }

        private void FillThemePaths(dynamic preloaderSetupJson, PreloaderSetup preloaderSetup)
        {
            foreach (dynamic themePathJson in preloaderSetupJson.themesPaths)
            {
                preloaderSetup.ThemesPaths.Add(CreateThemePath(themePathJson));
            }

        }

        private PreloaderSetup.ThemePath CreateThemePath(dynamic themePathJson)
        {
            return new PreloaderSetup.ThemePath()
            {
                Theme = ConvertDynamicValue<string>(themePathJson.theme),
                ConfigPath = ConvertDynamicValue<string>(themePathJson.configPath),
                PreloaderPath = ConvertDynamicValue<string>(themePathJson.preloaderPath),
                LobbyThemePath = ConvertDynamicValue<string>(themePathJson.lobbyThemePath),
                PreloaderLogoPath = ConvertDynamicValue<string>(themePathJson.preloaderLogoPath),
                LobbyThemePath_JS = ConvertDynamicValue<string>(themePathJson.lobbyThemePath_JS),
                PreloaderPath_JS = ConvertDynamicValue<string>(themePathJson.preloaderPath_JS)
            };
        }

        private void FillThemeEntries(dynamic preloaderSetupJson, PreloaderSetup preloaderSetup)
        {
            foreach(dynamic themeEntryJson in preloaderSetupJson.themeEntries)
            {
                preloaderSetup.ThemeEntries.Add(CreateThemeEntry(themeEntryJson));
            }
        }

        private PreloaderSetup.ThemeEntry CreateThemeEntry(dynamic themeEntryJson)
        {
            return new PreloaderSetup.ThemeEntry()
            {
                Brand = ConvertDynamicValue<string>(themeEntryJson.brand),
                Skin = ConvertDynamicValue<string>(themeEntryJson.skin),
                Language = ConvertDynamicValue<string>(themeEntryJson.language),
                ClientProfile = CreateClientProfile(themeEntryJson.clientProfile)
            };
        }

        private PreloaderSetup.ClientProfile CreateClientProfile(dynamic clientProfile)
        {
            return new PreloaderSetup.ClientProfile()
            {
                NavigationPlanPath = ConvertDynamicValue<string>(clientProfile.navigationPlanPath),
                ZippedResourcesPath = ConvertDynamicValue<string>(clientProfile.zippedResourcesPath),
                IconResources = ConvertDynamicValue<string>(clientProfile.iconResources),
                LanguagePath = ConvertDynamicValue<string>(clientProfile.languagePath),
                Theme = ConvertDynamicValue<string>(clientProfile.theme),
                Environmentset = ConvertDynamicValue<string>(clientProfile.environmentset),
                SimplePreloader = ConvertDynamicValue<string>(clientProfile.simplePreloader),
                DefaultSkin = ConvertDynamicValue<string>(clientProfile.defaultSkin),
                LanguageId = ConvertDynamicValue<string>(clientProfile.languageId)
            };
        }
    }
}
