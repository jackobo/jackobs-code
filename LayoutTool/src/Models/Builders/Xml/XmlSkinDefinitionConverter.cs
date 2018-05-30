using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using Spark.Infra.Types;

namespace LayoutTool.Models.Builders.Xml
{
    public class XmlSkinDefinitionConverter : IXmlSkinDefinitionConverter
    {

        IClientConfigurationFile _gamesProperties;
        private XDocument _gamesPropertiesXml;
        public IClientConfigurationFile GamesProperties
        {
            set
            {
                _gamesProperties = value;
                SetXml(ref _gamesPropertiesXml, _gamesProperties.Content);
            }
        }

        IClientConfigurationFile _iconResources;
        XDocument _iconResourcesXml;
        public IClientConfigurationFile IconResources
        {
            set
            {
                _iconResources = value;

                SetXml(ref _iconResourcesXml, _iconResources.Content);
            }
        }

        IClientConfigurationFile _gamesTexts;
        private XDocument _gamesTextsXml;
        public IClientConfigurationFile GamesTexts
        {

            set
            {
                _gamesTexts = value;
                SetXml(ref _gamesTextsXml, _gamesTexts.Content);
            }
        }


        private XDocument _skinXml;
        private IClientConfigurationFile _skin;
        public IClientConfigurationFile Skin
        {
            get
            {
                return _skin;
            }
            set
            {
                _skin = value;
                SetXml(ref _skinXml, _skin.Content);
            }
        }

        private XDocument _navigationPlanXml;
        IClientConfigurationFile _navigationPlan;
        public IClientConfigurationFile NavigationPlan
        {
            get
            {
                return _navigationPlan;
            }
            set
            {
                _navigationPlan = value;
                SetXml(ref _navigationPlanXml,  _navigationPlan.Content);
            }
        }
        

        private void SetXml(ref XDocument xmlProperty, string xml)
        {
            if (string.IsNullOrEmpty(xml))
                xmlProperty = null;
            else
                xmlProperty = XmlUtils.Parse(xml);
        }

        public SkinConversionResult Convert(SkinDefinition skinDefinition)
        {
            UpdateGamesGroup(skinDefinition.SkinContent.TopGames, XmlNames.topGames, XmlNames.topGame);
            UpdateGamesGroup(skinDefinition.SkinContent.VipTopGames, XmlNames.vipGames, XmlNames.vipGame);
            UpdateArenas(skinDefinition.SkinContent);
            UpdateMyAccount(skinDefinition.SkinContent);

            var result = new List<ConvertedClientConfigurationFile>();

            UpdateTriggers(skinDefinition.SkinContent.Triggers).Do(file => result.Add(file));

            result.Add(new ConvertedClientConfigurationFile(_navigationPlan, _navigationPlanXml.ToString()));
            
            var games = AppendMissingGames(skinDefinition, result);
            
            return new SkinConversionResult(result.ToArray(), games);
        }

        private Optional<ConvertedClientConfigurationFile> UpdateTriggers(TriggerCollection triggers)
        {
            XElement triggersElement = null;
            if (_skinXml != null)
            {
                triggersElement = _skinXml.Root.Element(XmlNames.triggers);

                if (triggersElement == null)
                {
                    triggersElement = new XElement(XmlNames.triggers);
                    _skinXml.Root.Add(triggersElement);
                }
            }
            else
            {
                triggersElement = _navigationPlanXml.Root.Element(XmlNames.triggers);

                if (triggersElement == null)
                {
                    triggersElement = new XElement(XmlNames.triggers);
                    _navigationPlanXml.Root.AddFirst(triggersElement);
                }
            }

            //remove the old triggers
            foreach(var triggerElement in triggersElement.Elements(XmlNames.trigger)
                                                        .Where(element => element.Elements(XmlNames.action)
                                                                                 .Any(actionElement => IsDynamicLayoutActionElement(actionElement)))
                                                        .ToArray())
            {
                triggerElement.Remove();
            }


            //add new triggers
            foreach(var trigger in triggers)
            {
                var triggerElement = new XElement(XmlNames.trigger);
                triggerElement.AddOrUpdateAttributeValue(XmlNames.name, trigger.Name);
                triggerElement.AddOrUpdateAttributeValue(XmlNames.priority, trigger.Priority);

                foreach (var action in trigger.Actions)
                {
                    var actionElement = new XElement(XmlNames.action);
                    actionElement.AddOrUpdateAttributeValue(XmlNames.name, action.Name);

                    var conditionsElement = new XElement(XmlNames.conditions);

                    actionElement.Add(conditionsElement);

                    foreach(var condition in action.Conditions)
                    {
                        var conditionElement = new XElement(XmlNames.condition);

                        conditionElement.AddOrUpdateAttributeValue(XmlNames.updateType, condition.UpdateType);
                        conditionElement.AddOrUpdateAttributeValue(XmlNames.type, condition.Type);
                        conditionElement.AddOrUpdateAttributeValue(XmlNames.equationType, condition.EquationType);

                        if(condition.Values.Count == 1)
                        {
                            conditionElement.AddOrUpdateAttributeValue(XmlNames.value, condition.Values.First().Value);
                        }
                        else
                        {
                            foreach (var value in condition.Values)
                            {
                                conditionElement.Add(new XElement(XmlNames.value, value.Value));
                            }
                        }
                        conditionsElement.Add(conditionElement);
                    }

                    triggerElement.Add(actionElement);
                }

                triggersElement.Add(triggerElement);
            }

            if (_skinXml != null)
                return Optional<ConvertedClientConfigurationFile>.Some(new ConvertedClientConfigurationFile(Skin, _skinXml.ToString()));
            else
                return Optional<ConvertedClientConfigurationFile>.None();
        }

        private bool IsDynamicLayoutActionElement(XElement actionElement)
        {
            return PlayerStatusType.DynamicLayouts.Any(dl => dl.ActionName == actionElement.GetAttributeValue(XmlNames.name));
        }

        private SkinConversionResult.NewGameInformation[] AppendMissingGames(SkinDefinition skinDefinition, List<ConvertedClientConfigurationFile> files)
        {

            if(_gamesPropertiesXml == null)
            {
                return new SkinConversionResult.NewGameInformation[0];
            }

            var newGames = new List<SkinConversionResult.NewGameInformation>();
            var iconResources = _iconResourcesXml.Root.Element("icons");
            var gamesTextsElements = _gamesTextsXml.Root.Element(XmlNames.elements);
            var gamesXmlElement = _gamesPropertiesXml.Root.Element(XmlNames.games);



            var existingGamesInGamesProperties = gamesXmlElement.Descendants(XmlNames.game).Select(element => element.GetAttributeValue<int>(XmlNames.gameType))
                                                               .Distinct()
                                                               .ToDictionary(gt => gt);



            

            foreach (var game in skinDefinition.SkinContent.Arenas.SelectMany(a => a.Layouts.SelectMany(l => l.Games))
                                             .GroupBy(g => g.GameType)
                                             .Select(g => g.First()))
            {
                if (existingGamesInGamesProperties.ContainsKey(game.GameType))
                    continue;

                var newGameXmlElement = new XElement(XmlNames.game);
                newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.gameType, game.GameType);
                newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.eventType, "GameLink");
                newGameXmlElement.AddOrUpdateAttributeValue("histGroup", 0);
                newGameXmlElement.AddOrUpdateAttributeValue("previewTemplateID", 0);
                newGameXmlElement.AddOrUpdateAttributeValue("groupId", 0);
                
                

                var resourcePrefix = game.Name.Replace(" ", "_") + "_" + game.GameType;
                newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.resourcePrefix, resourcePrefix);
                newGameXmlElement.AddOrUpdateAttributeValue("toolTipLabelId", resourcePrefix);
                
                newGameXmlElement.AddOrUpdateAttributeValue("gameTagLabelId", resourcePrefix);
                newGameXmlElement.AddOrUpdateAttributeValue("lobbyLabelId", resourcePrefix);
                newGameXmlElement.AddOrUpdateAttributeValue("tabLabelId", resourcePrefix);
                newGameXmlElement.AddOrUpdateAttributeValue("gameBarLabelId", resourcePrefix);

                

                gamesXmlElement.Add(newGameXmlElement);

                var gameTextElement = new XElement(XmlNames.element);
                gameTextElement.AddOrUpdateAttributeValue(XmlNames.varName, resourcePrefix);
                gameTextElement.Value = game.Name;

                gamesTextsElements.Add(gameTextElement);


                string relativeImageUrl = $"navigation/media/icons/{game.GameType}/image.png";
                var iconUrlElement = new XElement(XmlNames.item);
                iconUrlElement.AddOrUpdateAttributeValue("name", resourcePrefix);
                iconUrlElement.AddOrUpdateAttributeValue("source",  relativeImageUrl);
                iconResources.Add(iconUrlElement);

                iconUrlElement = new XElement(XmlNames.item);
                iconUrlElement.AddOrUpdateAttributeValue("name", resourcePrefix + "Preview");
                iconUrlElement.AddOrUpdateAttributeValue("source", relativeImageUrl);
                iconResources.Add(iconUrlElement);

                iconUrlElement = new XElement(XmlNames.item);
                iconUrlElement.AddOrUpdateAttributeValue("name", resourcePrefix + "Lobby");
                iconUrlElement.AddOrUpdateAttributeValue("source", relativeImageUrl);
                iconResources.Add(iconUrlElement);
                newGames.Add(new SkinConversionResult.NewGameInformation(game.GameType, game.Name, relativeImageUrl));
            }


            if(newGames.Count > 0)
            {
                files.Add(new ConvertedClientConfigurationFile(_gamesProperties, _gamesPropertiesXml.ToString()));
                files.Add(new ConvertedClientConfigurationFile(_gamesTexts, _gamesTextsXml.ToString()));
                files.Add(new ConvertedClientConfigurationFile(_iconResources, _iconResourcesXml.ToString()));
            }
            else
            {
                files.Add(new ConvertedClientConfigurationFile(_gamesProperties, null));
                files.Add(new ConvertedClientConfigurationFile(_gamesTexts, null));
                files.Add(new ConvertedClientConfigurationFile(_iconResources, null));
            }



            return newGames.ToArray();
        }

        private void UpdateMyAccount(SkinContent skinContent)
        {
            UpdateMyAccountSection(XmlValues.lobby, skinContent.MyAccountLobby);
            UpdateMyAccountSection(XmlValues.history, skinContent.MyAccountHistory);
        }

        private void UpdateMyAccountSection(string screen, MyAccountItemCollection newItems)
        {
            var oldMyAccountSection = lobby_data_ndl.Elements(XmlNames.myAccount).FirstOrDefault(element => element.GetAttributeValue(XmlNames.screen) == screen);

            var newMyAccountSection = new XElement(XmlNames.myAccount);

            if (oldMyAccountSection != null)
            {
                oldMyAccountSection.CopyAttributesTo(newMyAccountSection);
            }

            newMyAccountSection.AddOrUpdateAttributeValue(XmlNames.screen, screen);
            

            foreach (var item in newItems)
            {
                var itemElement = new XElement(XmlNames.item);

                foreach (var attr in item.Attributes)
                {
                    itemElement.Add(new XAttribute(attr.Name, attr.Value));
                }

                newMyAccountSection.Add(itemElement);
            }

            if(oldMyAccountSection != null)
            {
                oldMyAccountSection.AddBeforeSelf(newMyAccountSection);
                oldMyAccountSection.Remove();
            }
            else
            {
                lobby_data_ndl.Add(newMyAccountSection);
            }

        }

        private void UpdateGamesGroup(GameGroupLayoutCollection gameGroupLayouts, string parentElementName, string childElementName)
        {
            if (lobby_data_ndl == null)
                return;

            var originalGroupElements = lobby_data_ndl.Elements(parentElementName).ToArray();
         

            var arenasXmlElement = lobby_data_ndl.Element(XmlNames.arenas);

            foreach(var gameGroupLayout in gameGroupLayouts)
            {

                var originalGroupXmlElement = originalGroupElements.FirstOrDefault(e => 0 == string.Compare(e.GetAttributeValue(XmlNames.playerStatus, ""),  gameGroupLayout.PlayerStatus ?? ""));
                var newGroupXmlElement = new XElement(parentElementName);

                if (originalGroupXmlElement != null)
                    originalGroupXmlElement.CopyAttributesTo(newGroupXmlElement);
                

                newGroupXmlElement.AddOrUpdateAttributeValue(XmlNames.buildaction, XmlValues.copy);

                if (!string.IsNullOrEmpty(gameGroupLayout.PlayerStatus))
                {
                    newGroupXmlElement.AddOrUpdateAttributeValue(XmlNames.playerStatus, gameGroupLayout.PlayerStatus);
                }
                
                foreach (var game in gameGroupLayout.Games)
                {
                    newGroupXmlElement.Add(new XComment(game.Name));

                    var gameXmlElement = new XElement(childElementName);
                    gameXmlElement.Add(new XAttribute(XmlNames.gameType, game.Id));
                    newGroupXmlElement.Add(gameXmlElement);
                }


                if (originalGroupXmlElement != null)
                {
                    originalGroupXmlElement.AddBeforeSelf(newGroupXmlElement);
                    originalGroupXmlElement.Remove();
                }
                else
                {
                    arenasXmlElement.AddBeforeSelf(newGroupXmlElement);
                }
            }
        }

        private void UpdateArenas(SkinContent layout)
        {
            var existingArenasXmlElements = lobby_data_ndl.Element(XmlNames.arenas).Elements(XmlNames.arena).ToArray();
            var newAreansXmlElements = new List<XNode>();

            foreach (var arena in layout.Arenas)
            {
                foreach (var arenaLayout in arena.Layouts)
                {
                    newAreansXmlElements.Add(new XComment(arena.Name));
                    newAreansXmlElements.Add(CreateArenaXmlElement(existingArenasXmlElements, arena, arenaLayout));
                }
            }

            foreach (var lobby in layout.Lobbies)
            {
                newAreansXmlElements.Add(CreateLobbyArenaXmlElement(existingArenasXmlElements, lobby, layout.Arenas));
            }



            var arenasXmlElement = lobby_data_ndl.Element(XmlNames.arenas);
            var arenasXmlElementAttributes = arenasXmlElement.Attributes().ToArray();
            arenasXmlElement.ReplaceAll(newAreansXmlElements.ToArray());
            arenasXmlElement.Add(arenasXmlElementAttributes);
        }



        private XElement CreateLobbyArenaXmlElement(XElement[] existingArenasXmlElements, Lobby lobby, ArenaCollection arenas)
        {
            XElement existingLobbyArenaXmlElement = null;

            if (string.IsNullOrEmpty(lobby.PlayerStatus))
            {
                existingLobbyArenaXmlElement = existingArenasXmlElements.FirstOrDefault(element => element.GetAttributeValue(XmlNames.type) == XmlValues.lobby);
            }
            else
            {
                existingLobbyArenaXmlElement = existingArenasXmlElements.FirstOrDefault(element => element.GetAttributeValue(XmlNames.type) == XmlValues.lobby
                                                                                              && element.GetAttributeValue(XmlNames.playerStatus) == lobby.PlayerStatus);
            }
            

            XElement newLobbyArenaXmlElement = new XElement(XmlNames.arena);

            if (existingLobbyArenaXmlElement != null)
            {
                existingLobbyArenaXmlElement.CopyAttributesTo(newLobbyArenaXmlElement);
            }
            
            
            newLobbyArenaXmlElement.AddOrUpdateAttributeValue(XmlNames.type, XmlValues.lobby);
            

            if (!string.IsNullOrEmpty(lobby.PlayerStatus))
            {
                newLobbyArenaXmlElement.AddOrUpdateAttributeValue(XmlNames.playerStatus, lobby.PlayerStatus);
            }

            newLobbyArenaXmlElement.AddOrUpdateAttributeValue(XmlNames.favoritesSize, lobby.FavoriteSize);

            int placeHolder = 0;
            
            for (int i = 0; i < lobby.Items.Count; i++)
            {
                var lobbyItem = lobby.Items[i];

                
                
                var lobbyItemXmlElement = new XElement(XmlNames.game);
                lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.placeHolder, placeHolder.ToString().PadLeft(2, '0'));
                lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.gameType, lobbyItem.Id);
                if (lobbyItem.JackpotVisible/* || arenas.IsJackpotVisible(lobbyItem.Id, lobby.PlayerStatus)*/)
                {
                    lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.templateId, XmlValues.mcDynamicTextedButtonPersonalize);
                }
                else
                {
                    lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.templateId, XmlValues.btnDynamicTexted);
                }
                if (XmlValues.x.ContainsKey(lobby.Items.Count))
                {
                    lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.x, XmlValues.x[lobby.Items.Count][i]);
                }
                else
                {
                    var oneArenaWidth = 996 / lobby.Items.Count;
                    lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.x, oneArenaWidth * i + 1);
                }
                lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.y, XmlValues.y);
                lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.iconSize, XmlValues.BIG_ICON);
                var rect = ArenaGame.GameTypeToRect(lobbyItem.Id);
                if (!string.IsNullOrEmpty(rect))
                {
                    lobbyItemXmlElement.AddOrUpdateAttributeValue(XmlNames.rect, rect);
                }
                newLobbyArenaXmlElement.Add(lobbyItemXmlElement);

                placeHolder++;
            }

            return newLobbyArenaXmlElement;
        }

        private XElement CreateArenaXmlElement(XElement[] existingArenasXmlElements, Arena arena, ArenaLayout arenaLayout)
        {
            XElement existingArenaXmlElement = null;

            if (string.IsNullOrEmpty(arenaLayout.PlayerStatus))
            {
                existingArenaXmlElement = existingArenasXmlElements.FirstOrDefault(element => element.GetAttributeValue(XmlNames.type) == arena.Type.ToString());
            }
            else
            {
                existingArenaXmlElement = existingArenasXmlElements.FirstOrDefault(element => element.GetAttributeValue(XmlNames.type) == arena.Type.ToString()
                                                                                              && element.GetAttributeValue(XmlNames.playerStatus) == arenaLayout.PlayerStatus);
            }

            var newArenaXmlElement = new XElement(XmlNames.arena);


            if (existingArenaXmlElement != null)
            {
                existingArenaXmlElement.CopyAttributesTo(newArenaXmlElement);
                existingArenaXmlElement.CopyAllElementsExcept(newArenaXmlElement, XmlNames.filteringInfo, XmlNames.game, XmlNames.alsoPlayingGames);
            }
            else
            {
                //get de default arena element
                existingArenaXmlElement = existingArenasXmlElements.FirstOrDefault(element => element.GetAttributeValue(XmlNames.type) == arena.Type.ToString());
                if (existingArenaXmlElement != null)
                {
                    existingArenaXmlElement.CopyAllElementsExcept(newArenaXmlElement, XmlNames.filteringInfo, XmlNames.game, XmlNames.alsoPlayingGames);
                }

                newArenaXmlElement.AddOrUpdateAttributeValue(XmlNames.type, arena.Type);
                if (!string.IsNullOrEmpty(arenaLayout.PlayerStatus))
                {
                    newArenaXmlElement.AddOrUpdateAttributeValue(XmlNames.playerStatus, arenaLayout.PlayerStatus);
                }
            }
            
            newArenaXmlElement.AddOrUpdateAttributeValue(XmlNames.JPVisible, arenaLayout.JackpotVisible);
            if (arena.IsNewGameArena)
            {
                newArenaXmlElement.AddOrUpdateAttributeValue(XmlNames.newGame, "true");
            }
            
            UpdateFilteringInfo(newArenaXmlElement, arenaLayout.FilteringInfo);

            UpdateAlsoPlayingGames(newArenaXmlElement, arenaLayout.AlsoPlayingGames);

            UpdateArenaGames(existingArenaXmlElement, newArenaXmlElement, arenaLayout.Games);

            return newArenaXmlElement;
        }

        private void UpdateArenaGames(XElement existingArenaXmlElement, XElement newArenaXmlElement, ArenaGameCollection games)
        {
            var existingGamesXmlElements = new Dictionary<string, XElement>();

            if(existingArenaXmlElement != null)
            {
                foreach(var gameXmlElement in existingArenaXmlElement.Elements(XmlNames.game))
                {
                    var gameType = gameXmlElement.GetAttributeValue(XmlNames.gameType);
                    if(!existingGamesXmlElements.ContainsKey(gameType))
                    {
                        existingGamesXmlElements.Add(gameType, gameXmlElement);
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("Duplicated game " + gameType);
                    }
                }

                
            }

            
            foreach (var game in games)
            {


                XElement newGameXmlElement = null;
                if (existingGamesXmlElements.ContainsKey(game.GameType.ToString()))
                {
                    newGameXmlElement = existingGamesXmlElements[game.GameType.ToString()];
                }
                else
                {
                    newGameXmlElement = new XElement(XmlNames.game);
                    newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.gameType, game.GameType);
                    newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.templateId, XmlValues.btnArenaTexedTemplate);
                    newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.rect, ArenaGame.GameTypeToRect(game.GameType));
                }

                RemovePlaceHolderAttribute(newGameXmlElement);

              
                AddOrRemoveNewGameAttribute(game, newGameXmlElement);

                if (game.UserMode != UserModes.Both)
                {
                    newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.userMode, game.UserMode);
                }

                newArenaXmlElement.Add(new XComment(game.Name));
                newArenaXmlElement.Add(newGameXmlElement);
              

            }
        }

        private void RemovePlaceHolderAttribute(XElement gameXmlElement)
        {
            var placeHolderAttribute = gameXmlElement.Attribute("placeholder"); //in some navigation plan files this attribute name is miss spelled 
            if (placeHolderAttribute != null)
                placeHolderAttribute.Remove();


            placeHolderAttribute = gameXmlElement.Attribute(XmlNames.placeHolder);
            if (placeHolderAttribute != null)
                placeHolderAttribute.Remove();


        }


        private static void AddOrRemoveNewGameAttribute(ArenaGame game, XElement newGameXmlElement)
        {
            var newGameAttribute = newGameXmlElement.Attribute(XmlNames.newGame);

            if (game.NewGame)
            {
                if (newGameAttribute == null)
                {
                    newGameXmlElement.AddOrUpdateAttributeValue(XmlNames.newGame, "true");
                }
                else
                {
                    newGameAttribute.SetValue("true");
                }
            }
            else
            {
                if (newGameAttribute != null)
                {
                    newGameAttribute.Remove();
                }
            }
        }

       

        private void UpdateAlsoPlayingGames(XElement newArenaXmlElement, GameCollection alsoPlayingGames)
        {
            if (alsoPlayingGames.Count == 0)
                return;

            var alsoPlayingGamesXmlElement = new XElement(XmlNames.alsoPlayingGames);

            foreach(var game in alsoPlayingGames)
            {
                var alsoPlayingGameXmlElement = new XElement(XmlNames.alsoPlayingGame);
                alsoPlayingGamesXmlElement.Add(new XComment(game.Name));
                alsoPlayingGameXmlElement.AddOrUpdateAttributeValue(XmlNames.gameType, game.Id);
                alsoPlayingGamesXmlElement.Add(alsoPlayingGameXmlElement);
            }

            newArenaXmlElement.Add(alsoPlayingGamesXmlElement);
        }

        private void UpdateFilteringInfo(XElement newArenaXmlElement, FilterCollection filters)
        {


            var existingFilteringInfoElement = newArenaXmlElement.Element(XmlNames.filteringInfo);
            if (existingFilteringInfoElement != null)
                existingFilteringInfoElement.Remove();
            
            var newFilteringInfoXmlElement = new XElement(XmlNames.filteringInfo);
            
            foreach(var filter in filters)
            {
                var newFilterXmlElement = new XElement(XmlNames.filter);

                foreach(var a in filter.Attributes)
                {
                    newFilterXmlElement.Add(new XAttribute(a.Name, a.Value));
                }

                newFilteringInfoXmlElement.Add(newFilterXmlElement);
            }

            newArenaXmlElement.Add(newFilteringInfoXmlElement);
        }


        XElement lobby_data_ndl
        {
            get
            {
                return _navigationPlanXml.Root.Element(XmlNames.lobby_data_ndl);
            }
        }

     
    }
}
