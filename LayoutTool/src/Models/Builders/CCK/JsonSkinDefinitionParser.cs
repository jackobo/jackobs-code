using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spark.Infra.Logging;
using static LayoutTool.Models.Builders.CCK.DynamicsUtils;
using static LayoutTool.Models.Builders.CCK.JsonUtils;

namespace LayoutTool.Models.Builders.CCK
{
//https://mirage-ndl.888sport.com/Mirage/Version_7.4/application/versions.json
//https://mirage-ndl.888sport.com/Mirage/Version_7.4/application/preloader_setup.json
//https://mirage-ndl.888sport.com/Mirage/Version_7.4/7.4-0-87/bin/application/ViewActual.html?antiCacheKey=1472541578554&entrancemode=1&navigationplanid=-1&languageid=en&brandid=0&brandname=888casino&skinid=4&isdebugmode=1&isSelectedDebugHost=0&GRSState=LATEST&GRSAdditionalState=LATEST&GRSGameTechnology=DEFAULT&CommunicationType=socketProxy&Environment=MIRAGE&preloaderSetupPath=../../../application/preloader_setup.json&ref=https%3A%2F%2Fmirage-ndl.888sport.com%2FMirage%2FVersion_7.4%2Fapplication%2Findex.html
    public class JsonSkinDefinitionParser : IJsonSkinDefinitionParser
    {
        public JsonSkinDefinitionParser(ILoggerFactory loggerFactory)
        {
            this.Logger = loggerFactory.CreateLogger(this.GetType());
        }

        ILogger Logger { get; set; }

        Dictionary<string, string> _texts;
        
        public IClientConfigurationFile LanguageJson
        {
            set
            {

                _texts = new Dictionary<string, string>();

                try
                {
                    var languageJson = JsonConvert.DeserializeObject<dynamic>(value.Content);

                    AppendTexts(value, languageJson.lobby);
                    AppendTexts(value, languageJson.games);
                }
                catch(Exception ex)
                {
                    throw new ApplicationException($"Failed to parse language file {value.Location}", ex);
                }
            }
        }

        private void AppendTexts(IClientConfigurationFile value, dynamic properties)
        {
            foreach (dynamic property in properties)
            {
                if (!_texts.ContainsKey(property.Name))
                {

                    if (HasProperty(property, JsonNames.text))
                    {
                        _texts.Add(property.Name, ConvertDynamicValue<string>(property.Value.text));
                    }
                    else
                    {
                        //Logger.Warning($"Missing '{JsonNames.text}' property langauge item '{property.Name}' in file {value.Location}");
                    }

                }
            }
        }

        dynamic _navigationPlanJson;
        public IClientConfigurationFile NavigationPlanJson
        {
            set
            {
                try
                {
                    _navigationPlanJson = JsonConvert.DeserializeObject<dynamic>(value.Content);
                }
                catch (Exception ex)
                {
                    throw new ApplicationException($"Failed to parse navigation plan json file {value.Location}", ex);
                }
            }
        }

        public SkinDefinitionContext Parse()
        {
            var skinDefinitionContext = new SkinDefinitionContext();
            skinDefinitionContext.AvailableArenaTypes = ReadAvailableArenaTypes();
            skinDefinitionContext.AvailableGames = ReadAvailableGames();
            skinDefinitionContext.AvailableFilters = ReadAvailableFilters();


            skinDefinitionContext.SkinDefinition = new SkinDefinition();

            skinDefinitionContext.SkinDefinition.SkinContent = ReadSkinContent(skinDefinitionContext);

            return skinDefinitionContext;
        }

        private FilterCollection ReadAvailableFilters()
        {
#warning for CCK the only option is to hard code the available filters maybe we can change this in the future
            var filters = new FilterCollection();

            filters.Add(CreateAvailableFilter("FILTER_ROULETTE", "ROULETTE", "roulette"));
            filters.Add(CreateAvailableFilter("FILTER_PRIVATE", "PRIVATE", "privateTable"));
            filters.Add(CreateAvailableFilter("FILTER_PUBLIC", "PUBLIC", "publicTable"));
            filters.Add(CreateAvailableFilter("FILTER_VIP", "VIP", "vipGame"));
            filters.Add(CreateAvailableFilter("FILTER_BLACKJACK", "BLACKJACK", "blackJack"));
            filters.Add(CreateAvailableFilter("FILTER_MULTIPLAIERS", "MULTIPLAIERS", "publicTable"));
            //filters.Add(CreateAvailableFilter("FILTER_HIGHEST_RATED", "HIGHEST RATED", "hightRares"));
            filters.Add(CreateAvailableFilter("FILTER_LINESET1TO5", "1-5 LINES", "lineSetOne"));
            filters.Add(CreateAvailableFilter("FILTER_LINE_SET_1_20", "1-20 LINES", "lineSetTwo"));
            filters.Add(CreateAvailableFilter("FILTER_LINESET25PLUS", "25+ LINES", "lineSetThree"));
            filters.Add(CreateAvailableFilter("FILTER_JACKPOT", "JACKPOT", "progressiveJackpot"));
            filters.Add(CreateAvailableFilter("FILTER_NEWGAME", "NEW", "newGame"));
            filters.Add(CreateAvailableFilter("FILTER_VIDEOPOKER", "VIDEO POKER", "videoPoker"));
            filters.Add(CreateAvailableFilter("FILTER_SPORTGAME", "SPORTS", "sportGame"));
            filters.Add(CreateAvailableFilter("FILTER_HILOGAME", "HI-LO", "hiLoGame"));
            filters.Add(CreateAvailableFilter("FILTER_POKER", "POKER", "poker"));
            filters.Add(CreateAvailableFilter("FILTER_CLASSICSLOT", "CLASSIC SLOTS", "classicSlot"));
            filters.Add(CreateAvailableFilter("FILTER_VIDEOSLOT", "VIDEO SLOTS", "videoSlot"));
            filters.Add(CreateAvailableFilter("FILTER_CARDGAME", "CARD GAMES", "cardGame"));
            filters.Add(CreateAvailableFilter("FILTER_TABLEGAME", "TABLE GAME", "tableGame"));
            filters.Add(CreateAvailableFilter("FILTER_LIVEGAME", "LIVE", "liveGame"));
            filters.Add(CreateAvailableFilter("FILTER_CRAPSGAME", "CRAPS", "crapsGame"));
            filters.Add(CreateAvailableFilter("FILTER_KENOGAME", "KENO", "kenoGame"));
            filters.Add(CreateAvailableFilter("FREESPINGAME", "FreePlay GAMES", "freeSpinGame"));
            filters.Add(CreateAvailableFilter("FILTER_ROULETTEZEROS", "ZEROS", "rouletteZeros"));
            filters.Add(CreateAvailableFilter("FILTER_WIDEGAME", "WIDE GAME", "wideGame"));
            filters.Add(CreateAvailableFilter("FILTER_BACCARAT", "BACCARAT", "baccaratGame"));
            filters.Add(CreateAvailableFilter("FILTER_ALL_PAYS", "ALL PAYS", "allPays"));
            
            return filters;
        }

        private Filter CreateAvailableFilter(string label, string name, string field, string value = "true")
        {
            return new Filter(label, name,
                              new AttributeValue("label", label),
                              new AttributeValue("field", field),
                              new AttributeValue("value", value));
        }

        private SkinContent ReadSkinContent(SkinDefinitionContext skinDefinitionContext)
        {
            SkinContent layout = new SkinContent();

            FillSkinContent(layout, skinDefinitionContext);

            return layout;
        }

        private void FillSkinContent(SkinContent skinContent, SkinDefinitionContext skinDefinitionContext)
        {
            FillArenasCollection(skinContent.Arenas, skinDefinitionContext);
            FillLobbyCollection(skinContent.Lobbies, skinDefinitionContext);
            FillGamesGroupCollection(skinContent.TopGames, skinDefinitionContext, _navigationPlanJson.navigationPlan.topGames);
            FillGamesGroupCollection(skinContent.VipTopGames, skinDefinitionContext, _navigationPlanJson.navigationPlan.vipGames);

            FillMyAccountCollection(skinContent.MyAccountLobby, _navigationPlanJson.navigationPlan.menus.lobby);
            FillMyAccountCollection(skinContent.MyAccountHistory, _navigationPlanJson.navigationPlan.menus.history);

            FillTriggers(skinContent.Triggers);

        }

        private void FillTriggers(TriggerCollection triggersCollection)
        {
            dynamic triggersElement = _navigationPlanJson?.triggersStorage?.triggers;
            if (triggersElement == null)
                return;


            foreach(var triggerElement in triggersElement)
            {
                string actionName = ConvertDynamicValue<string>(triggerElement.actionName);

                if (!TriggerAction.POSSIBLE_ACTION_NAMES.Contains(actionName))
                    continue;

                Trigger trigger = new Trigger(ConvertDynamicValue<string>(triggerElement.name),
                                              ConvertDynamicValue<int>(triggerElement.priority));

                var action = new TriggerAction(actionName);

                foreach(var conditionElement in triggerElement.conditions)
                {
                    var condition = new Condition(ConvertDynamicValue<string>(conditionElement.updateType),
                                                  ConvertDynamicValue<string>(conditionElement.type),
                                                  ConvertDynamicValue<string>(conditionElement.equationType));
                    
                    foreach(var value in conditionElement.values)
                    {
                        condition.Values.Add(new ConditionValue(ConvertDynamicValue<string>(value)));
                    }

                    action.Conditions.Add(condition);
                }

                trigger.Actions.Add(action);
                triggersCollection.Add(trigger);
            }
            
        }

        private void FillMyAccountCollection(MyAccountItemCollection myAccountCollection, dynamic myAccountJsonCollection)
        {
            if (myAccountJsonCollection == null)
                return;


            foreach (var accountItemElement in myAccountJsonCollection)
            {
                var id = ConvertDynamicValue<string>(accountItemElement.id);

                myAccountCollection.Add(new MyAccountItem(id, this.GetElementFriendlyName(id), JsonUtils.ExtractAllAttributes((JObject)accountItemElement)));
            }
        }


        private string GetElementFriendlyName(string id)
        {
            if (_texts.ContainsKey(id))
                return _texts[id];
            

            return id;

        }

        private void FillGamesGroupCollection(GameGroupLayoutCollection gamesGroupsLayoutCollection, SkinDefinitionContext skinDefinitionContext, dynamic gamesGroup)
        {
            if (gamesGroup == null)
                return;

            var gameGroupLayout = new GameGroupLayout();
            foreach (var gameType in gamesGroup)
            {
                gameGroupLayout.Games.Add(skinDefinitionContext.GetGame(ConvertDynamicValue<int>(gameType)));
            }

            gamesGroupsLayoutCollection.Add(gameGroupLayout);
        }

        private void FillLobbyCollection(LobbyCollection lobbyCollection, SkinDefinitionContext skinDefinitionContext)
        {

            foreach (var lobbyElement in _navigationPlanJson.navigationPlan.arenas)
            {
                if (JsonValues.lobby != ConvertDynamicValue<string>(lobbyElement.type))
                    continue;

                var lobby = new Lobby(ConvertDynamicValue<int>(lobbyElement.favoritesSize),
                                      ExtractPlayerStatus(lobbyElement.playerStatus));

                foreach (var lobbyItemElement in lobbyElement.games)
                {
                    lobby.Items.Add(new LobbyItem(ConvertDynamicValue<int>(lobbyItemElement.gametype), 
                                                  ConvertDynamicValue<string>(lobbyItemElement.templateId) == JsonValues.mcDynamicTextedButtonPersonalize));
                }

                lobbyCollection.Add(lobby);
            }
        }

        private void FillArenasCollection(ArenaCollection arenasCollection, SkinDefinitionContext skinDefinitionContext)
        {
            var arenasElementsByType = GetArenasGroupedByType();

            foreach (var arenaGroup in arenasElementsByType)
            {
                var arenaType = skinDefinitionContext.GetArenaType(arenaGroup.Key);

                var arena = new Arena(arenaType.Id, arenaType.Name, arenaGroup.Value.Any(element => ConvertDynamicValue<bool>(element.newGame) == true));

                foreach (var arenaLayoutElement in arenaGroup.Value)
                {

                    //make sure the JPVisible attribute is always there
                    var attributes = JsonUtils.ExtractAllAttributes((JObject)arenaLayoutElement);
                    if(!attributes.Any(a => a.Name == JsonNames.jpVisible))
                    {
                        attributes.Add(new AttributeValue(JsonNames.jpVisible, "false"));
                    }

                    var arenaLayout = new ArenaLayout(ExtractPlayerStatus(arenaLayoutElement.playerStatus), attributes);
                    
                    FillArenaLayout(arenaLayout, arenaLayoutElement, skinDefinitionContext);

                    arena.Layouts.Add(arenaLayout);
                }

                arenasCollection.Add(arena);
            }
        }


        private string ExtractPlayerStatus(dynamic playerStatus)
        {
            if (playerStatus == null)
                return string.Empty;

            var array = (JArray)playerStatus;

            if (array.Count == 0)
                return string.Empty;

            return ConvertDynamicValue<string>(array.First());
        }

        private void FillArenaLayout(ArenaLayout arenaLayout, dynamic arenaLayoutElement, SkinDefinitionContext skinDefinitionContext)
        {
            FillArenaLayoutGames(arenaLayout, arenaLayoutElement, skinDefinitionContext);
            FillArenaLayoutGridInfo(arenaLayout, arenaLayoutElement);
            FillArenaLayoutFilters(arenaLayout, arenaLayoutElement, skinDefinitionContext);
            FillArenaLayoutAlsoPlayingGames(arenaLayout, arenaLayoutElement, skinDefinitionContext);
        }

        private void FillArenaLayoutGridInfo(ArenaLayout arenaLayout, dynamic arenaLayoutElement)
        {
            var dataGridArenaInfoElement = arenaLayoutElement.dataGridArenaInfo;
            if (dataGridArenaInfoElement == null)
                return;
            
            foreach (dynamic columnElement in dataGridArenaInfoElement)
            {
                arenaLayout.DataGridInfo.Add(new ArenaGridColumn(JsonUtils.ExtractAllAttributes(columnElement)));
            }
        }

        private void FillArenaLayoutAlsoPlayingGames(ArenaLayout arenaLayout, dynamic arenaLayoutElement, SkinDefinitionContext skinDefinitionContext)
        {
            var alsoPlayingGamesElement = arenaLayoutElement.alsoPlayingGames;
            if (alsoPlayingGamesElement == null)
                return;

            foreach (var alsoPlayingGameElement in alsoPlayingGamesElement)
            {
                arenaLayout.AlsoPlayingGames.Add(skinDefinitionContext.GetGame(ConvertDynamicValue<int>(alsoPlayingGameElement)));
            }
        }

        private void FillArenaLayoutFilters(ArenaLayout arenaLayout, dynamic arenaLayoutElement, SkinDefinitionContext skinDefinitionContext)
        {
            var filteringInfoElement = arenaLayoutElement.filteringInfo;
            if (filteringInfoElement == null)
                return;


            foreach (var filterElement in filteringInfoElement)
            {

                var label = ConvertDynamicValue<string>(filterElement.label);
                var filter = skinDefinitionContext.GetFilter(label);

                if (filter == null)
                {
                  
                    filter = new Filter(label, 
                                        GetArenaFilterName(filterElement), 
                                        ((JObject)filterElement).Properties().Select(p => new AttributeValue(p.Name, p.Value.ToString())));
                    
                    skinDefinitionContext.AvailableFilters.Add(filter);
                }

                arenaLayout.FilteringInfo.Add(filter);

            }
        }

        private string GetArenaFilterName(dynamic filterElement)
        {
            var label = ConvertDynamicValue<string>(filterElement.label);

            if (_texts.ContainsKey(label))
                return _texts[label];
            else
                return label.Replace("FILTER_", "");
        }
    

        private void FillArenaLayoutGames(ArenaLayout arenaLayout, dynamic arenaLayoutElement, SkinDefinitionContext skinDefinitionContext)
        {
            foreach (var gameElement in arenaLayoutElement.games)
            {
                var gameType = ConvertDynamicValue<int>(gameElement.gametype);

                var game = skinDefinitionContext.GetGame(gameType);

                ArenaGame arenaGame = new ArenaGame(game.Id,
                                                    game.Name,
                                                    ConvertDynamicValue<bool>(gameElement.newGame, false),
                                                    ConvertDynamicValue<int>(gameElement.userMode, UserModes.Both));


                arenaLayout.Games.Add(arenaGame);


            }
        }

       

        private Dictionary<int, List<dynamic>> GetArenasGroupedByType()
        {
            var arenasElementsByType = new Dictionary<int, List<dynamic>>();
            foreach (var arena in _navigationPlanJson.navigationPlan.arenas)
            {
                if (JsonValues.lobby == ConvertDynamicValue<string>(arena.type))
                    continue;

                int arenaType = ConvertDynamicValue<int>(arena.type);

                if (!arenasElementsByType.ContainsKey(arenaType))
                    arenasElementsByType.Add(arenaType, new List<dynamic>());

                arenasElementsByType[arenaType].Add(arena);

            }

            return arenasElementsByType;
        }

        private ArenaTypeCollection ReadAvailableArenaTypes()
        {
            var result = new ArenaTypeCollection();
            foreach(dynamic game in _navigationPlanJson.navigationPlan.games.properties.gamesPropertiesList)
            {
                if(game.Value.eventType == JsonValues.ArenaLink)
                {
                    result.Add(new ArenaType(int.Parse((string)game.Value.gameType), GetGameOrArenaFriendlyName(game)));
                }

            }

            return result;
        }


        private string GetGameOrArenaFriendlyName(dynamic game)
        {
            string key = (string)game.Value.lobbyLabelId;
            if (string.IsNullOrEmpty(key))
            {
                key = (string)game.Value.gameHistoryLabelId;
            }

            if (string.IsNullOrEmpty(key))
                return null;

            if (_texts.ContainsKey(key))
                return _texts[key];
            else
                return key;

        }

        private GameCollection ReadAvailableGames()
        {
            var result = new GameCollection();

            foreach (dynamic game in _navigationPlanJson.navigationPlan.games.properties.gamesPropertiesList)
            {
                var eventType = game.Value.eventType ?? "";
                if (eventType != JsonValues.ArenaLink && eventType != "") 
                {
                    result.Add(new Game(int.Parse((string)game.Value.gameType), GetGameOrArenaFriendlyName(game)));
                }

            }

            return result;
        }
    }
}
