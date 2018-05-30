using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using static LayoutTool.Models.Builders.CCK.DynamicsUtils;
using static LayoutTool.Models.Builders.CCK.JsonUtils;

namespace LayoutTool.Models.Builders.CCK
{
    public class JsonSkinDefinitionConverter : IJsonSkinDefinitionConverter
    {
        IClientConfigurationFile _language;
        public IClientConfigurationFile Language
        {
            set
            {
                _language = value;
            }
        }

        IClientConfigurationFile _navigationPlan;
        public IClientConfigurationFile NavigationPlan
        {
            set
            {
                _navigationPlan = value;
            }
        }

        public SkinConversionResult Convert(SkinDefinition skinDefinition)
        {
            var navigationPlanJson = JsonConvert.DeserializeObject<dynamic>(_navigationPlan.Content);
            var languageJson = JsonConvert.DeserializeObject<dynamic>(_language.Content);


            UpdateGamesGroup(skinDefinition.SkinContent.TopGames, navigationPlanJson.navigationPlan, JsonNames.topGames);
            UpdateGamesGroup(skinDefinition.SkinContent.VipTopGames, navigationPlanJson.navigationPlan, JsonNames.vipGames);
            UpdateArenas(skinDefinition.SkinContent, navigationPlanJson.navigationPlan);
            UpdateMyAccount(skinDefinition.SkinContent, navigationPlanJson);
            UpdateTriggers(skinDefinition.SkinContent.Triggers, navigationPlanJson);

            var resultedFiles = new List<ConvertedClientConfigurationFile>();
            var newGames = AppendMissingGames(skinDefinition, navigationPlanJson, languageJson);

            return new SkinConversionResult(new ConvertedClientConfigurationFile[]
            {
                new ConvertedClientConfigurationFile(_navigationPlan, JsonConvert.SerializeObject(navigationPlanJson)),
                new ConvertedClientConfigurationFile(_language, JsonConvert.SerializeObject(languageJson))
            }
            , newGames);
            
        }

        private void UpdateTriggers(TriggerCollection triggers, dynamic navigationPlanJson)
        {
            var existingTriggers = navigationPlanJson.triggersStorage.triggers;

            //remove the old triggers
            foreach (var triggerElement in ((IEnumerable)existingTriggers).Cast<dynamic>().Where(triggerElement => IsDynamicLayoutActionElement(triggerElement))
                                                        .ToArray())
            {
                triggerElement.Remove();
            }


            //add new triggers
            foreach (var trigger in triggers.OrderBy(t => t.Priority))
            {
                dynamic triggerElement = new JObject();
                triggerElement.name = trigger.Name;
                triggerElement.priority = trigger.Priority;
                triggerElement.actionType = "";

                foreach (var action in trigger.Actions)
                {
                    triggerElement.actionName = action.Name; //It looks like I'm setting it here multiple times but actually there is only one action

                    var conditionsArray = new JArray();
                    triggerElement.conditions = conditionsArray;

                    foreach (var condition in action.Conditions)
                    {
                        dynamic conditionElement = new JObject();
                        conditionElement.type = condition.Type;
                        conditionElement.updateType = condition.UpdateType;
                        conditionElement.equationType = condition.EquationType;

                        var valuesArray = new JArray();
                        conditionElement.values = valuesArray;
                        foreach (var value in condition.Values)
                        {
                            valuesArray.Add(new JValue(value.Value));
                        }
                        
                        conditionsArray.Add(conditionElement);
                    }

                    
                }
                existingTriggers.Add(triggerElement);
            }
        }

        private bool IsDynamicLayoutActionElement(dynamic trigger)
        {
            return PlayerStatusType.DynamicLayouts.Any(dl => dl.ActionName == ConvertDynamicValue<string>(trigger.actionName));
        }


        private void UpdateMyAccount(SkinContent skinContent, dynamic navigationPlanJson)
        {
            navigationPlanJson.navigationPlan.menus.lobby = GenerateMyAccountSection(skinContent.MyAccountLobby);
            navigationPlanJson.navigationPlan.menus.history = GenerateMyAccountSection(skinContent.MyAccountHistory);
        }

        private JArray GenerateMyAccountSection(MyAccountItemCollection myAccountSection)
        {
            
          
            var result = new JArray();
            foreach(var item in myAccountSection)
            {
                var obj = new JObject();

                foreach(var attr in item.Attributes)
                {
                    obj.Add(new JProperty(attr.Name, attr.Value));
                }

                result.Add(obj);
            }

            return result;

        }

        private SkinConversionResult.NewGameInformation[] AppendMissingGames(SkinDefinition skinDefinition, dynamic navigationPlanJson, dynamic languageJson)
        {
            var gamesProperties = (JObject)navigationPlanJson.navigationPlan.games.properties.gamesPropertiesList;

            var existingGames = new Dictionary<int, JToken>();

            foreach(JProperty prop in gamesProperties.Properties() )
            {
                existingGames.Add(int.Parse(prop.Name), prop.Value);
            }

            var newGames = new List<SkinConversionResult.NewGameInformation>();
            foreach(var game in skinDefinition.SkinContent.Arenas.SelectMany(a => a.Layouts.SelectMany(l => l.Games)))
            {
                if (existingGames.ContainsKey(game.GameType))
                    continue;

                var resourcePrefix = game.Name.Replace(" ", "_").Replace(".", "_") + "_" + game.GameType;
                var relativeImageUrl = $"navigation/media/icons/{game.GameType}/image.png";

                newGames.Add(new SkinConversionResult.NewGameInformation(game.GameType, game.Name, relativeImageUrl));

                AddToGamesModuleStorageIfNecessary(navigationPlanJson, game);

                dynamic newGameElement = new JObject();
                newGameElement.blockedGame = new JArray(); //
                newGameElement.bonusGame = false;//
                newGameElement.eventType = "GameLink";
                newGameElement.freeSpinGame = false;//
                newGameElement.gameBarLabelId = resourcePrefix;
                newGameElement.gameHistoryLabelId = resourcePrefix;
                newGameElement.gameTagLabelId = resourcePrefix;
                newGameElement.gameType = game.GameType;
                newGameElement.groupId = 30;
                newGameElement.height = false;
                newGameElement.histGroup = 12;
                newGameElement.historyDDLLabelId = resourcePrefix;
                newGameElement.images = new JArray();//
                newGameElement.jackpotId = 6006;//
                newGameElement.lineSetThree = false;//
                newGameElement.lobbyLabelId = resourcePrefix;
                
                newGameElement.maxBetDKK = 300000;//
				newGameElement.maxBetEUR= 30000;//
				newGameElement.maxBetGBP= 30000;//
				newGameElement.maxBetUSD= 30000;//
				newGameElement.maxPerSpotEUR= 1000;//
				newGameElement.maxPerSpotGBP= 1000;//
				newGameElement.maxPerSpotUSD= 1000;//
				newGameElement.minBetDKK = 10;//
				newGameElement.minBetEUR= 1;//
				newGameElement.minBetGBP= 1;//
				newGameElement.minBetUSD= 1;//
				newGameElement.mobileLabel= false;//
				newGameElement.multipliers= false;//
				newGameElement.numOfDecks= 0;//
				newGameElement.numOfLines= 30;//
				newGameElement.numOfReels= 5;//
                newGameElement.previewTemplateID = 0;
                newGameElement.privateTable = false;//
                newGameElement.progressiveJackpot = false;//
                newGameElement.resourcePrefix = resourcePrefix;
                newGameElement.rouletteZeros = 0;//
                newGameElement.standardGame = true;//
                newGameElement.styles = new JArray();//
                newGameElement.tabLabelId = resourcePrefix;
                newGameElement.toolTipLabelId = resourcePrefix;
                newGameElement.videoSlot = true; //
                newGameElement.width = false;//
                newGameElement.wrapperType = "GGPGAME";//
                

                gamesProperties.Add(new JProperty(game.GameType.ToString(), newGameElement));


                var iconResources = (JObject)navigationPlanJson.navigationPlan.iconResources.images;
                iconResources.Add(new JProperty(resourcePrefix, new JValue(relativeImageUrl)));
                iconResources.Add(new JProperty(resourcePrefix + "Preview", new JValue(relativeImageUrl)));
                iconResources.Add(new JProperty(resourcePrefix + "Lobby", new JValue(relativeImageUrl)));
                iconResources.Add(new JProperty(resourcePrefix + "Small", new JValue(relativeImageUrl)));
                iconResources.Add(new JProperty(resourcePrefix + "FullBackground", new JValue(relativeImageUrl)));


                var gamesTexts = (JObject)languageJson.games;
                dynamic newGameTexts = new JObject();
                newGameTexts.html = false;
                newGameTexts.text = game.Name;
                gamesTexts.Add(new JProperty(resourcePrefix, newGameTexts));


            }

            return newGames.ToArray();
        }

        private static void AddToGamesModuleStorageIfNecessary(dynamic navigationPlanJson, ArenaGame game)
        {
            var gamesModule = (JArray)navigationPlanJson.gameModulesStorage.gameModule.games;
            
            foreach(JObject g in gamesModule)
            {
                if (g.Property("id").Value.ToString() == game.GameType.ToString())
                    return;
            }

            dynamic newGameModule = new JObject();
            newGameModule.id = game.GameType;
            newGameModule.component = "modernGame";
            gamesModule.Add(newGameModule);
        }

        private void UpdateArenas(SkinContent skinContent, dynamic navigationPlanElement)
        {
            var arenasElement = navigationPlanElement.arenas;

            var existingArenasElements = ((System.Collections.IEnumerable)arenasElement).Cast<dynamic>().ToArray();

            var newAreansElements = new JArray();


            foreach (var arena in skinContent.Arenas)
            {
                foreach (var arenaLayout in arena.Layouts)
                {
                    newAreansElements.Add(CreateArenaElement(existingArenasElements, arena, arenaLayout));
                }
            }


            foreach (var lobby in skinContent.Lobbies)
            {
                newAreansElements.Add(CreateLobbyArenaElement(lobby, skinContent.Arenas));
            }

            

            navigationPlanElement.arenas = newAreansElements;

        }

        private dynamic CreateLobbyArenaElement(Lobby lobby, ArenaCollection arenas)
        {


            dynamic newLobbyArenaElement = new JObject();
        

            newLobbyArenaElement.playerStatus = BuildPlayerStatusArray(lobby.PlayerStatus);
            newLobbyArenaElement.arenaLabelId = "";
            newLobbyArenaElement.type = JsonValues.lobby;
            newLobbyArenaElement.jpVisible = false;
            
            newLobbyArenaElement.favoritesSize = lobby.FavoriteSize;
            
            var lobbyItemsArray = new JArray();

            for (int i = 0; i < lobby.Items.Count; i++)
            {
                var lobbyItem = lobby.Items[i];



                dynamic lobbyItemXmlElement = new JObject();
                lobbyItemXmlElement.arenaType = JsonValues.lobby;
                lobbyItemXmlElement.arenaTextName = "";
                lobbyItemXmlElement.gametype = lobbyItem.Id;
                lobbyItemXmlElement.iconSizeName = JsonValues.BIG_ICON;
                lobbyItemXmlElement.isGameHasImageStates = false;
                lobbyItemXmlElement.displayMode = 3;
                lobbyItemXmlElement.userMode = 3;

                if (lobbyItem.JackpotVisible /*|| arenas.IsJackpotVisible(lobbyItem.Id, lobby.PlayerStatus)*/)
                {
                    lobbyItemXmlElement.templateId = JsonValues.mcDynamicTextedButtonPersonalize;
                }
                else
                {
                    lobbyItemXmlElement.templateId = JsonValues.btnDynamicTexted;
                }

                lobbyItemXmlElement.rectId = ArenaGame.GameTypeToRect(lobbyItem.Id);

                dynamic iconParamsElement = new JObject();
                if (JsonValues.x.ContainsKey(lobby.Items.Count))
                {
                    iconParamsElement.x = JsonValues.x[lobby.Items.Count][i];
                }
                else
                {
                    var oneArenaWidth = 996 / lobby.Items.Count;
                    iconParamsElement.x = oneArenaWidth * i + 1;
                }
                iconParamsElement.y = JsonValues.y;
                iconParamsElement.width = 166;
                iconParamsElement.height = 200;
                lobbyItemXmlElement.iconParams = iconParamsElement;

                lobbyItemsArray.Add(lobbyItemXmlElement);
            }

            newLobbyArenaElement.games = lobbyItemsArray;

            return newLobbyArenaElement;
        }

        private dynamic CreateArenaElement(dynamic[] existingArenasElements, Arena arena, ArenaLayout arenaLayout)
        {

            JObject existingArenaElement = null;

            if (string.IsNullOrEmpty(arenaLayout.PlayerStatus))
            {
                existingArenaElement = existingArenasElements.FirstOrDefault(element =>  ConvertDynamicValue<string>(element.type) == arena.Type.ToString())
                                            as JObject;
            }
            else
            {
                existingArenaElement = existingArenasElements.FirstOrDefault(element => ConvertDynamicValue<string>(element.type) == arena.Type.ToString()
                                                                                              && ExtractPlayerStatus(element.playerStatus) == arenaLayout.PlayerStatus)
                                          as JObject;
                                          
            }

            dynamic newArenaElement = null;

            
            if (existingArenaElement != null)
            {
                newArenaElement = existingArenaElement;
            }
            else
            {
                //get de default arena element
                existingArenaElement = existingArenasElements.FirstOrDefault(element => ConvertDynamicValue<string>(element.type) == arena.Type.ToString());
                if (existingArenaElement != null)
                {
                    newArenaElement = existingArenaElement.DeepClone();
                }
                else
                {
                    newArenaElement = new JObject();
                }
            }

            newArenaElement.type = arena.Type;
            newArenaElement.playerStatus = BuildPlayerStatusArray(arenaLayout.PlayerStatus);
            //newArenaElement.favoritesSize = arenaLayout.FavoritesSize;
            newArenaElement.jpVisible = arenaLayout.JackpotVisible;
            newArenaElement.newGame = arena.IsNewGameArena;

            UpdateFilteringInfo(arenaLayout.FilteringInfo, newArenaElement);
            UpdateAlsoPlayingGames(newArenaElement, arenaLayout.AlsoPlayingGames);
            UpdateArenaGames(arenaLayout.Games, newArenaElement);

            return newArenaElement;

        }

        private void UpdateArenaGames(ArenaGameCollection games, dynamic newArenaElement)
        {
            var gamesArray = new JArray();

            foreach(var game in games)
            {
                dynamic gameElement = new JObject();

                gameElement.arenaType = newArenaElement.type;
                gameElement.arenaTextName = "";
                gameElement.gametype = game.GameType;
                gameElement.isGameHasImageStates = false;
                gameElement.displayMode = 3;
                gameElement.templateId = JsonValues.btnArenaTexedTemplate;
                gameElement.rectId = ArenaGame.GameTypeToRect(game.GameType);

                gamesArray.Add((JObject)gameElement);
            }

            newArenaElement.games = gamesArray;
        }

        private void UpdateAlsoPlayingGames(dynamic newArenaElement, GameCollection alsoPlayingGames)
        {
            var alsoPlayingGamesArray = new JArray();
            foreach(var game in alsoPlayingGames)
            {
                alsoPlayingGamesArray.Add(new JValue(game.Id));
            }

            newArenaElement.alsoPlayingGames = alsoPlayingGamesArray;
        }

        private void UpdateFilteringInfo(FilterCollection filters, dynamic newArenaElement)
        {
            var filteringInfo = new JArray();

            foreach(var filter in filters)
            {
                var filterElement = new JObject();
                foreach(var atribute in filter.Attributes)
                {
                    filterElement.Add(new JProperty(atribute.Name, new JValue(atribute.Value)));
                }

                filteringInfo.Add(filterElement);
            }

            newArenaElement.filteringInfo = filteringInfo;
        }

        private void UpdateGamesGroup(GameGroupLayoutCollection topGames, dynamic navigationPlanJson, string groupName)
        {
            
            var topGamesDefaultLayout = topGames.FirstOrDefault(group => string.IsNullOrEmpty(group.PlayerStatus));

            
            var obj = navigationPlanJson as JObject;

            var topGamesArray = new JArray();
            if (topGamesDefaultLayout != null)
            {
                foreach (var game in topGamesDefaultLayout.Games)
                {
                    topGamesArray.Add(new JValue(game.Id));
                }
            }


            var prop = obj.Property(groupName);

            if(prop == null)
            {
                prop = new JProperty(groupName, topGamesArray);
                obj.Add(prop);
            }
            else
            {
                prop.Value = topGamesArray;
            }
            
        }
    }
}
