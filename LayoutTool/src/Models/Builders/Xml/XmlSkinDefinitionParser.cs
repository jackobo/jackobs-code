using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.Models.Builders.Xml
{
    public class XmlSkinDefinitionParser : IXmlSkinDefinitionParser
    {
        public XmlSkinDefinitionParser()
        {

        }


        private Dictionary<string, string> _gamesTexts = new Dictionary<string, string>();


        public string GamesTextsXml
        {
            set
            {
                _gamesTexts = ReadTexts(value);
            }
        }

        private Dictionary<string, string> _lobbyTexts = new Dictionary<string, string>();

        public string LobbyTextsXml
        {
            set
            {
                _lobbyTexts = ReadTexts(value);
            }
        }

        private Dictionary<string, string> ReadTexts(string xml)
        {
            XDocument xmlDoc = null;

            SetXml(ref xmlDoc, xml);

            var dic = new Dictionary<string, string>();
            if (xmlDoc != null)
            {
                foreach (var xmlElement in xmlDoc.Root.Element(XmlNames.elements).Elements(XmlNames.element))
                {
                    if (string.IsNullOrEmpty(xmlElement.Value))
                        continue;

                    var key = xmlElement.GetAttributeValue(XmlNames.varName);

                    if (string.IsNullOrEmpty(key))
                        continue;

                    if (dic.ContainsKey(key))
                        continue;


                    dic.Add(key, xmlElement.Value.Trim().Replace("\\n", " "));

                }
            }

            return dic;

        }


        private void SetXml(ref XDocument xmlProperty, string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                xmlProperty = null;
            }
            else
            {
                xmlProperty = XmlUtils.Parse(xml);
            }
        }

        private XDocument _gamesPropertiesXml;
        public string GamesPropertiesXml
        {
           
            set
            {
                SetXml(ref _gamesPropertiesXml, value);
            }
        }

        /*
        private XDocument _brandXml;
        public string BrandXml
        {
            set { SetXml(ref _brandXml, value); }
        }
        */


        private XDocument _skinXml;
        public string SkinXml
        {
            set { SetXml(ref _skinXml, value); }
        }


        private XDocument _navigationPlanXml;
        public string NavigationPlanXml
        {
           
            set
            {
                SetXml(ref _navigationPlanXml, value);
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
        
        private void FillMyAccountCollection(MyAccountItemCollection myAccountCollection, string screen)
        {
            var myAccountElement = _navigationPlanXml.Root.Element(XmlNames.lobby_data_ndl)
                                   ?.Elements(XmlNames.myAccount)
                                   ?.Where(element => element.GetAttributeValue(XmlNames.screen) == screen)
                                   .FirstOrDefault();

            if (myAccountElement == null)
                return;

            foreach (var accountItemElement in myAccountElement.Elements(XmlNames.item))
            {
                var id = accountItemElement.GetAttributeValue(XmlNames.id);
                myAccountCollection.Add(new MyAccountItem(id, this.GetElementFriendlyName(id), accountItemElement.ExtractAllAttributes()));
            }
        }

        private SkinContent ReadSkinContent(SkinDefinitionContext skinDefinitionContext)
        {
            if (_navigationPlanXml == null)
                return null;
            
            SkinContent layout = new SkinContent();

            FillSkinContent(layout, skinDefinitionContext);
            
            return layout;
        }


      

        private void FillSkinContent(SkinContent skinContent, SkinDefinitionContext skinDefinitionContext)
        {
            FillArenasCollection(skinContent.Arenas, skinDefinitionContext);
            FillLobbyCollection(skinContent.Lobbies, skinDefinitionContext);
            FillGamesGroupCollection(skinContent.TopGames, skinDefinitionContext, XmlNames.topGames, XmlNames.topGame);
            FillGamesGroupCollection(skinContent.VipTopGames, skinDefinitionContext, XmlNames.vipGames, XmlNames.vipGame);

            FillMyAccountCollection(skinContent.MyAccountLobby, XmlValues.lobby);
            FillMyAccountCollection(skinContent.MyAccountHistory, XmlValues.history);

            FillTriggers(skinContent.Triggers);

        }

        private void FillTriggers(TriggerCollection triggers)
        {

            var triggersElement = _skinXml.Root.Element(XmlNames.triggers);
            if (triggersElement == null)
                return;

            foreach (var triggerElement in triggersElement.Elements(XmlNames.trigger))
            {
                var actionsElements = triggerElement.Elements(XmlNames.action).Where(e => TriggerAction.POSSIBLE_ACTION_NAMES.Contains(e.GetAttributeValue(XmlNames.name))).ToArray();
                if (actionsElements.Length == 0)
                    continue;

                var trigger = new Trigger(triggerElement.GetAttributeValue(XmlNames.name),
                                          triggerElement.GetAttributeValue<int>(XmlNames.priority));


                
                
                foreach(var actionElement in actionsElements)
                {
                    var action = new TriggerAction(actionElement.GetAttributeValue(XmlNames.name));
                    trigger.Actions.Add(action);
                        
                    var conditionsElement = actionElement.Element(XmlNames.conditions);
                    if (conditionsElement == null)
                        continue;

                    foreach(var conditionElement in conditionsElement.Elements(XmlNames.condition))
                    {
                        var condition = new Condition(conditionElement.GetAttributeValue(XmlNames.updateType),
                                                        conditionElement.GetAttributeValue(XmlNames.type),
                                                        conditionElement.GetAttributeValue(XmlNames.equationType));
                        condition.UpdateType = conditionElement.GetAttributeValue(XmlNames.updateType);
                        condition.Type = conditionElement.GetAttributeValue(XmlNames.type);
                        condition.EquationType = conditionElement.GetAttributeValue(XmlNames.equationType);

                        var valueAttribute = conditionElement.Attribute(XmlNames.value);

                        if (valueAttribute != null)
                        {
                            condition.Values.Add(new ConditionValue(valueAttribute.Value));
                        }
                        else
                        {
                            foreach(var valueElement in conditionElement.Elements(XmlNames.value))
                            {
                                condition.Values.Add(new ConditionValue(valueElement.Value));
                            }
                        }


                        action.Conditions.Add(condition);
                    }
                }

                triggers.Add(trigger);

            }


        }

        private void FillGamesGroupCollection(GameGroupLayoutCollection gameGroupLayoutCollection, SkinDefinitionContext skinDefinitionContext, string xmlCollectionElementName, string xmlItemElementName)
        {
            var gamesGroupsElements = _navigationPlanXml.Root.Element(XmlNames.lobby_data_ndl)
                                            ?.Elements(xmlCollectionElementName)
                                            .ToArray();
                                       

            

            foreach (var gameGroupXmlElement in gamesGroupsElements)
            {
                var gameGroupLayout = new GameGroupLayout(gameGroupXmlElement.GetAttributeValue(XmlNames.playerStatus));

                
                foreach(var itemXmlElement in gameGroupXmlElement.Elements(xmlItemElementName))
                {
                    gameGroupLayout.Games.Add(skinDefinitionContext.GetGame(itemXmlElement.GetAttributeValue<int>(XmlNames.gameType)));
                }
                
                gameGroupLayoutCollection.Add(gameGroupLayout);

            }
        }

        private void FillLobbyCollection(LobbyCollection lobbyCollection, SkinDefinitionContext skinDefinitionContext)
        {
            var lobbyXmlElements = ReadArenasXmlElements().Where(element => XmlValues.lobby == element.GetAttributeValue(XmlNames.type));

            foreach(var lobbyXmlElement in lobbyXmlElements)
            {
                var lobby = new Lobby(lobbyXmlElement.GetAttributeValue<int>(XmlNames.favoritesSize),
                                      lobbyXmlElement.GetAttributeValue(XmlNames.playerStatus));

                foreach(var lobbyItemXmlElement in lobbyXmlElement.Elements(XmlNames.game))
                {
                    lobby.Items.Add(new LobbyItem(lobbyItemXmlElement.GetAttributeValue<int>(XmlNames.gameType),
                                                  XmlValues.mcDynamicTextedButtonPersonalize == lobbyItemXmlElement.GetAttributeValue (XmlNames.templateId, "")));
                }

                lobbyCollection.Add(lobby);
            }

        }

        private void FillArenasCollection(ArenaCollection arenasCollection, SkinDefinitionContext skinDefinitionContext)
        {
            var arenasElementsByType = ReadArenasXmlElements().Where(element => XmlValues.lobby != element.GetAttributeValue(XmlNames.type))
                                                        .GroupBy(element => element.GetAttributeValue<int>(XmlNames.type))
                                                        .ToDictionary(g => g.Key, g => g.ToList());
            
            foreach (var arenaGroup in arenasElementsByType)
            {
                var arenaType = skinDefinitionContext.GetArenaType(arenaGroup.Key);
                
                var arena = new Arena(arenaType.Id, arenaType.Name, arenaGroup.Value.Any(element => element.GetAttributeValue(XmlNames.newGame, false) == true));

                foreach(var arenaXmlElement in arenaGroup.Value)
                {

                    //make sure the JPVisible attribute is always there
                    var attributes = arenaXmlElement.ExtractAllAttributes();
                    if (!attributes.Any(a => a.Name == XmlNames.JPVisible))
                    {
                        attributes.Add(new AttributeValue(XmlNames.JPVisible, "false"));
                    }

                    var arenaLayout = new ArenaLayout(arenaXmlElement.GetAttributeValue(XmlNames.playerStatus), 
                                                      attributes);

                    

                    FillArenaLayout(arenaLayout, arenaXmlElement, skinDefinitionContext);

                    arena.Layouts.Add(arenaLayout);
                }
                
                

                arenasCollection.Add(arena);
            }

        }
        
        private IEnumerable<XElement> ReadArenasXmlElements()
        {
            if (_navigationPlanXml == null)
                return new XElement[0];

            var result = _navigationPlanXml.Root.Element(XmlNames.lobby_data_ndl)?.Element(XmlNames.arenas)?.Elements(XmlNames.arena).ToArray();

            if (result == null)
                return new XElement[0];

            return result;
        }

      

        private void FillArenaLayout(ArenaLayout arenaLayout, XElement arenaXmlElement, SkinDefinitionContext skinDefinitionContext)
        {

            FillArenaLayoutGames(arenaLayout, arenaXmlElement, skinDefinitionContext);
            FillArenaLayoutGridInfo(arenaLayout, arenaXmlElement);
            FillArenaLayoutFilters(arenaLayout, arenaXmlElement, skinDefinitionContext);
            FillArenaLayoutAlsoPlayingGames(arenaLayout, arenaXmlElement, skinDefinitionContext);


        }

        private void FillArenaLayoutGridInfo(ArenaLayout arenaLayout, XElement arenaXmlElement)
        {
            var gridInfoXmlElement = arenaXmlElement.Element(XmlNames.dataGridInfo);
            if (gridInfoXmlElement == null)
                return;
            
            foreach (var gridColumnElement in gridInfoXmlElement.Elements(XmlNames.column))
            {
                arenaLayout.DataGridInfo.Add(new ArenaGridColumn(gridColumnElement.ExtractAllAttributes()));
            }
        }

        private void FillArenaLayoutAlsoPlayingGames(ArenaLayout arenaLayout, XElement arenaXmlElement, SkinDefinitionContext skinDefinitionContext)
        {
            var alsoPlayingGamesXmlElement = arenaXmlElement.Element(XmlNames.alsoPlayingGames);
            if (alsoPlayingGamesXmlElement == null)
                return;

            foreach (var alsoPlayingGameXmlElement in alsoPlayingGamesXmlElement.Elements(XmlNames.alsoPlayingGame))
            {
                arenaLayout.AlsoPlayingGames.Add(skinDefinitionContext.GetGame(alsoPlayingGameXmlElement.GetAttributeValue<int>(XmlNames.gameType)));
            }

        }

        private void FillArenaLayoutGames(ArenaLayout arenaLayout, XElement arenaXmlElement, SkinDefinitionContext skinDefinitionContext)
        {


            foreach (var gameXmlElement in arenaXmlElement.Elements(XmlNames.game))
            {
                var gameType = gameXmlElement.GetAttributeValue<int>(XmlNames.gameType);

                var game = skinDefinitionContext.GetGame(gameType);

                ArenaGame arenaGame = new ArenaGame(game.Id,
                                                    game.Name,
                                                    gameXmlElement.GetAttributeValue<bool>(XmlNames.newGame, false),
                                                    gameXmlElement.GetAttributeValue(XmlNames.userMode, UserModes.Both));


                arenaLayout.Games.Add(arenaGame);


            }


        }

        private void FillArenaLayoutFilters(ArenaLayout arenaLayout, XElement arenaXmlElement, SkinDefinitionContext skinDefinitionContext)
        {
            var filteringInfoXmlElement = arenaXmlElement.Element(XmlNames.filteringInfo);
            if (filteringInfoXmlElement == null)
                return;


            foreach (var filterXmlElement in filteringInfoXmlElement.Elements(XmlNames.filter))
            {

                var label = filterXmlElement.GetAttributeValue(XmlNames.label);
                var filter = skinDefinitionContext.GetFilter(label);

                if(filter == null)
                {
                    
                    filter = new Filter(label, 
                                        GetArenaFilterName(filterXmlElement),
                                        filterXmlElement.Attributes().Select(a => new AttributeValue(a.Name.LocalName, a.Value)));
                    skinDefinitionContext.AvailableFilters.Add(filter);
                }
                
                arenaLayout.FilteringInfo.Add(filter);
                
            }


        }

        private FilterCollection ReadAvailableFilters()
        {
            if (_navigationPlanXml == null)
                return new FilterCollection();

            return new FilterCollection(_navigationPlanXml.Root.Element(XmlNames.lobby_data_ndl)
                                   ?.Elements(XmlNames.filter)
                                   .Select(element => new Filter(element.GetAttributeValue(XmlNames.label), 
                                                                 GetArenaFilterName(element),
                                                                 element.ExtractAllAttributes()))
                                   .ToList());
        }

        private string GetArenaFilterName(XElement element)
        {
            var label = element.GetAttributeValue(XmlNames.label);

            if (_lobbyTexts.ContainsKey(label))
                return _lobbyTexts[label];
            else
                return label.Replace("FILTER_", "");
        }

        private GameCollection ReadAvailableGames()
        {
            if (_gamesPropertiesXml == null)
                return new GameCollection();
            return new GameCollection(_gamesPropertiesXml.Root.Element(XmlNames.games).Descendants(XmlNames.game).Where(element => element.GetAttributeValue(XmlNames.eventType) != null && element.GetAttributeValue(XmlNames.eventType) != XmlNames.ArenaLink)
                                                  .Select(element => new Game(element.GetAttributeValue<int>(XmlNames.gameType),
                                                                              GetGameOrArenaFriendlyName(element)))
                                                  .ToArray());


        }

        private ArenaTypeCollection ReadAvailableArenaTypes()
        {
            if (_gamesPropertiesXml == null)
                return new ArenaTypeCollection();
            return new ArenaTypeCollection(_gamesPropertiesXml.Root.Element(XmlNames.games).Elements(XmlNames.game).Where(element => element.GetAttributeValue(XmlNames.eventType) == XmlNames.ArenaLink)
                                                    .Select(element => new ArenaType(element.GetAttributeValue<int>(XmlNames.gameType),
                                                                                     GetGameOrArenaFriendlyName(element)))
                                                    .ToArray());

        }

        private string GetGameOrArenaFriendlyName(XElement element)
        {

            var lobbyLabelId = element.GetAttributeValue(XmlNames.lobbyLabelId) ?? string.Empty;

            if (_gamesTexts.ContainsKey(lobbyLabelId))
                return _gamesTexts[lobbyLabelId];

            return element.GetAttributeValue(XmlNames.resourcePrefix);
        }

        private string GetElementFriendlyName(string id)
        {
            if (_lobbyTexts.ContainsKey(id))
                return _lobbyTexts[id];

            if (_gamesTexts.ContainsKey(id))
                return _gamesTexts[id];


            return id;

        }
    }


    

    
}
