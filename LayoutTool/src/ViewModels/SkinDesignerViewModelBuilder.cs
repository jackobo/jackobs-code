using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.Interfaces.Entities;
using LayoutTool.ViewModels.DynamicLayout;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;

namespace LayoutTool.ViewModels
{
    public class SkinDesignerViewModelBuilder
    {
        public SkinDesignerViewModelBuilder(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;
        
        List<AvailableGameViewModel> _existingClientGames;

        List<ErrorMessageViewModel> _buildErrors = new List<ErrorMessageViewModel>();

        public SkinDesignerViewModel Build(SkinDefinitionContext skinDefinitionContext)
        {
            _buildErrors = new List<ErrorMessageViewModel>();

            LoadAvailableGames(skinDefinitionContext.DestinationSkin.BrandId, 
                               skinDefinitionContext.SkinDefinition.SkinContent, 
                               skinDefinitionContext.AvailableGames);

            LoadAvailableFilters(skinDefinitionContext.AvailableFilters);
            LoadSkinDefinition(skinDefinitionContext.SkinDefinition);
            LoadExplorerBarItems();
            
            return new SkinDesignerViewModel(_serviceLocator,
                                             new AvailableGameViewModelCollection(_existingClientGames,
                                                                                            nameof(AvailableGameViewModel.GameType),
                                                                                            nameof(AvailableGameViewModel.Name),
                                                                                            nameof(AvailableGameViewModel.VendorName)),
                                             new AvailableGameViewModelCollection(_allOtherGames,
                                                                                            nameof(AvailableGameViewModel.GameType),
                                                                                            nameof(AvailableGameViewModel.Name),
                                                                                            nameof(AvailableGameViewModel.VendorName)),
                                             new FilterableCollectionViewModel<ArenaFilterViewModel>(_availableFilters, nameof(ArenaFilterViewModel.Name)),

                                             _skinDefinition,
                                             _explorerBar,
                                             _buildErrors,
                                             skinDefinitionContext.SourceSkin,
                                             skinDefinitionContext.DestinationSkin);
        }

        ExplorerBarViewModel _explorerBar;
        private void LoadExplorerBarItems()
        {
            
            _explorerBar = new ExplorerBarViewModel(_serviceLocator);
            _explorerBar.Items.Add(new ArenaCollectionExplorerBarItem(_skinDefinition.Arenas, _serviceLocator) { IsExpanded = true });
            _explorerBar.Items.Add(new LobbyCollectionExplorerBarItem(_skinDefinition.Lobbies, _serviceLocator));
            _explorerBar.Items.Add(new GameGroupLayoutCollectionExplorerBarItem(_skinDefinition.TopGames, _serviceLocator));
            _explorerBar.Items.Add(new GameGroupLayoutCollectionExplorerBarItem(_skinDefinition.VipGames, _serviceLocator));
            _explorerBar.Items.Add(new MyAccountExplorerBarItem(_skinDefinition.MyAccount, _serviceLocator));
            _explorerBar.Items.Add(new TriggersExplorerBarItem(_skinDefinition.Triggers, _serviceLocator));

        }

        SkinDefinitionViewModel _skinDefinition;
        private void LoadSkinDefinition(SkinDefinition skinDefinition)
        {
            _skinDefinition = new SkinDefinitionViewModel(_serviceLocator);
            
            var skinContent = skinDefinition.SkinContent;

            LoadTriggers(skinContent.Triggers);

            LoadArenas(skinContent.Arenas);
            LoadGamesGroupCollection("Top games", skinContent.TopGames, _skinDefinition.TopGames);
            LoadGamesGroupCollection("VIP games", skinContent.VipTopGames, _skinDefinition.VipGames);
            LoadLobbies(skinContent.Lobbies);
            LoadMyAccount(skinContent);
            

        }

        private void LoadTriggers(TriggerCollection triggers)
        {
            foreach(var trigger in triggers.OrderBy(t => t.Priority))
            {
                var action = trigger.Actions.FirstOrDefault();
                if (action == null)
                    continue;

                var playerStatus = PlayerStatusType.All.FirstOrDefault(t => t.ActionName == action.Name);
                var triggerViewModel = new TriggerViewModel(trigger.Name, new PlayerStatusTypeViewModel(playerStatus, _skinDefinition.Triggers), _serviceLocator);

                foreach(var condition in action.Conditions)
                {
                    var field = ConditionField.All.FirstOrDefault(f => f.CanBuildViewModel(condition));

                    if (field != null)
                    {
                        triggerViewModel.Conditions.Add(field.BuildConditionViewModel(condition, _serviceLocator));
                    }
                }

                _skinDefinition.Triggers.Add(triggerViewModel);
            }
        }

        private void LoadMyAccount(SkinContent skin)
        {
            var myAccountViewModel = new MyAccountViewModel();

            LoadMyAccountCollection(skin.MyAccountLobby, myAccountViewModel.Lobby);
            LoadMyAccountCollection(skin.MyAccountHistory, myAccountViewModel.History);

            foreach(var item in myAccountViewModel.Lobby.Union(myAccountViewModel.History).Distinct().OrderBy(item => item.Name))
            {
                myAccountViewModel.AllMyAccountItems.Add(item);
            }
            
            _skinDefinition.MyAccount = myAccountViewModel;
            
        }

        private void LoadMyAccountCollection(MyAccountItemCollection source, MyAccountItemCollectionViewModel destination)
        {
            foreach (var item in source)
            {
                destination.Add(new MyAccountItemViewModel(item.Id, item.Name, item.Attributes));
            }
        }

        
        private void LoadGamesGroupCollection(string description, GameGroupLayoutCollection source, GameGroupLayoutCollectionViewModel destination)
        {
            foreach (var layout in source)
            {
                var viewModel = new GameGroupLayoutViewModel(new PlayerStatusTypeViewModel(PlayerStatusType.FromId(layout.PlayerStatus), _skinDefinition.Triggers));

                foreach (var game in layout.Games)
                {
                    AddGameToCollection(game.Id, viewModel.Games, description);
                }

                destination.Add(viewModel);
            }
        }

        private void AddGameToCollection(int gameType, IList<AvailableGameViewModel> collection, string errorSourceName)
        {
            var availableGame = FindAvailableGameViewModel(gameType);
            if (availableGame != null)
            {
                collection.Add(availableGame);
            }
            else
            {
                _buildErrors.Add(new ErrorMessageViewModel(errorSourceName, $"Missing game {gameType}", ErrorServerity.Warning, null));
            }
        }

        private void LoadLobbies(LobbyCollection lobbies)
        {
            if(lobbies.Count == 0)
                _skinDefinition.Lobbies = new LobbyCollectionViewModel(Constants.DefaultNumberOfLobbyItems, _skinDefinition.Triggers);
            else
                _skinDefinition.Lobbies = new LobbyCollectionViewModel(lobbies.First().Items.Count, _skinDefinition.Triggers);

            foreach (var lobby in lobbies)
            {
                var lobbyViewModel = new LobbyViewModel(new PlayerStatusTypeViewModel(PlayerStatusType.FromId(lobby.PlayerStatus), _skinDefinition.Triggers), 
                                                        lobby.FavoriteSize);

                foreach (var item in lobby.Items)
                {
                    var itemViewModel = FindLobbyItemSource(item.Id);
                    if (itemViewModel != null)
                        lobbyViewModel.Items.Add(new LobbyItemViewModel(itemViewModel));
                    else
                        _buildErrors.Add(new ErrorMessageViewModel("Lobby arena", 
                                                           $"Cannot find the lobby item {item.Id} in the list of available lobby items", 
                                                           ErrorServerity.Warning,
                                                           null));
                }

                _skinDefinition.Lobbies.Add(lobbyViewModel);
            }
        }

        public ILobbyItemSource FindLobbyItemSource(int id)
        {
            var arenaViewModel = _skinDefinition.Arenas.FirstOrDefault(a => a.Type == id);

            if (arenaViewModel != null)
                return arenaViewModel;
            return FindAvailableGameViewModel(id);
        }


        private void LoadArenas(ArenaCollection arenas)
        {
            _skinDefinition.Arenas = new ArenaCollectionViewModel(_serviceLocator);
            foreach (var arena in arenas)
            {
                var arenaViewModel = new ArenaViewModel(arena.Type, arena.Name, arena.IsNewGameArena, _skinDefinition.Triggers, _serviceLocator);

                LoadArenaLayouts(arena, arenaViewModel);

                _skinDefinition.Arenas.Add(arenaViewModel);
            }

        }

        private void LoadArenaLayouts(Arena arena, ArenaViewModel arenaViewModel)
        {
            foreach (var arenaLayout in arena.Layouts)
            {
                var arenaLayoutViewModel = new ArenaLayoutViewModel(arenaViewModel,
                                                                    new PlayerStatusTypeViewModel(PlayerStatusType.FromId(arenaLayout.PlayerStatus), _skinDefinition.Triggers),
                                                                    arenaLayout.Attributes,
                                                                    arenaLayout.DataGridInfo);

                LoadArenaLayoutAlsoPlayingGames(arenaViewModel.Name, arenaLayoutViewModel, arenaLayout.AlsoPlayingGames);
                LoadArenaLayoutFilters(arenaLayoutViewModel, arenaLayout.FilteringInfo);
                LoadArenaLayoutGames(arenaViewModel.Name, arenaLayoutViewModel, arenaLayout.Games, arenaViewModel.IsNewGamesArena);

                arenaViewModel.Layouts.Add(arenaLayoutViewModel);
            }

            if (arenaViewModel.Layouts.Count > 0)
                arenaViewModel.Layouts[0].Activate();
        }

        private void LoadArenaLayoutGames(string arenaName, ArenaLayoutViewModel arenaLayoutViewModel, IEnumerable<ArenaGame> games, bool isNewGameArena)
        {
            
            foreach (var g in games)
            {
                var availableGameViewModel = FindAvailableGameViewModel(g.GameType);
                if (availableGameViewModel != null)
                {
                    arenaLayoutViewModel.Games.Add(new ArenaGameViewModel(availableGameViewModel, arenaLayoutViewModel.Games));
                }
                else
                {
                    _buildErrors.Add(new ErrorMessageViewModel(arenaName + " " + arenaLayoutViewModel.PlayerStatus.ToString(), 
                                                      $"Cannot find the game {g.GameType} in the list of available games", 
                                                      ErrorServerity.Warning,
                                                      null));
                }
            }
        }

        private void LoadArenaLayoutFilters(ArenaLayoutViewModel arenaLayoutViewModel, IEnumerable<Filter> availableFilters)
        {
            foreach (var f in availableFilters)
            {
                arenaLayoutViewModel.Filters.Add(GetArenaFilterViewModel(f.Id));
            }
        }


        private void LoadArenaLayoutAlsoPlayingGames(string arenaName, ArenaLayoutViewModel arenaLayoutViewModel, GameCollection alsoPlayingGames)
        {
            foreach (var g in alsoPlayingGames)
            {
                AddGameToCollection(g.Id, arenaLayoutViewModel.AlsoPlayingGames, arenaName + " " + arenaLayoutViewModel.PlayerStatus);
            }
        }

        private void LoadAvailableGames(int brandId, SkinContent layout, IEnumerable<Game> availableGames)
        {

            var gamesInfo = _serviceLocator.GetInstance<IGamesInformationProvider>()
                                                    .GetGamesInfo(brandId)
                                                    .ToDictionary(g => g.GameType);

            LoadExistingClientGames(layout, availableGames, gamesInfo);
            LoadAllOtherGames(gamesInfo);
        }

        private void LoadExistingClientGames(SkinContent layout, IEnumerable<Game> availableGames, Dictionary<int, GameInfo> gamesInfo)
        {
            var uniqueAvailableGames = layout.Arenas.SelectMany(a => a.Layouts.SelectMany(l => l.Games))
                                                             .GroupBy(g => g.GameType)
                                                             .ToDictionary(g => g.Key, g => new AvailableGameViewModel(g.Key,
                                                                                                         g.First().Name,
                                                                                                         ExtractGameProperty(gamesInfo, g.Key, item => item.GameGroup),
                                                                                                         g.Select(item => item.NewGame).Where(item => item == true).FirstOrDefault(),
                                                                                                         g.Max(item => item.UserMode),
                                                                                                         ExtractGameProperty(gamesInfo, g.Key, item => item.IsApproved),
                                                                                                         ExtractGameProperty(gamesInfo, g.Key, item => item.VendorName),
                                                                                                         ExtractGameProperty(gamesInfo, g.Key, item => item.JackpotIds),
                                                                                                         _serviceLocator));


            _existingClientGames = new List<AvailableGameViewModel>();

            foreach (var game in availableGames.OrderBy(g => g.Name))
            {
                if (uniqueAvailableGames.ContainsKey(game.Id))
                {
                    _existingClientGames.Add(uniqueAvailableGames[game.Id]);
                }
                else
                {
                    _existingClientGames.Add(new AvailableGameViewModel(game.Id,
                                                                        game.Name,
                                                                        ExtractGameProperty(gamesInfo, game.Id, item => item.GameGroup),
                                                                        false,
                                                                        UserModes.Both,
                                                                        ExtractGameProperty(gamesInfo, game.Id, item => item.IsApproved),
                                                                        ExtractGameProperty(gamesInfo, game.Id, item => item.VendorName),
                                                                        ExtractGameProperty(gamesInfo, game.Id, item => item.JackpotIds),
                                                                        _serviceLocator));
                }
            }
        }


        List<AvailableGameViewModel> _allOtherGames;

        private void LoadAllOtherGames(Dictionary<int, GameInfo> gamesInfo)
        {
#warning maybe I should notify somehow the user that there are duplicated games defined in the games_properties.xml.
            var existingClientGamesDictionary = _existingClientGames.Select(g => g.GameType).Distinct().ToDictionary(gt => gt);

            _allOtherGames = new List<AvailableGameViewModel>();

            foreach (var gameInfo in gamesInfo.OrderBy(item => item.Value.Name))
            {
                if (existingClientGamesDictionary.ContainsKey(gameInfo.Key))
                    continue;

                _allOtherGames.Add(new AvailableGameViewModel(gameInfo.Key,
                                                        gameInfo.Value.Name,
                                                        gameInfo.Value.GameGroup,
                                                        false,
                                                        UserModes.Both,
                                                        gameInfo.Value.IsApproved,
                                                        gameInfo.Value.VendorName,
                                                        gameInfo.Value.JackpotIds,
                                                        _serviceLocator));
            }
            
        }

        


        private T ExtractGameProperty<T>(Dictionary<int, GameInfo> gamesApprovalStatus, int gameType, Func<GameInfo, T> propertyValueExtractor)
        {
            if (gamesApprovalStatus.ContainsKey(gameType))
                return propertyValueExtractor(gamesApprovalStatus[gameType]);
            else
                return default(T);
        }

        List<ArenaFilterViewModel> _availableFilters;
        private void LoadAvailableFilters(IEnumerable<Filter> availableFilters)
        {
            _availableFilters = new List<ArenaFilterViewModel>();
            foreach (var filter in availableFilters.OrderBy(f => f.Name))
            {
                _availableFilters.Add(new ArenaFilterViewModel(filter.Id, filter.Name, filter.Attributes.ToArray()));
            }

            
        }


        public AvailableGameViewModel FindAvailableGameViewModel(int gameType)
        {
            var availableGame = _existingClientGames.FirstOrDefault(g => g.GameType == gameType);
            
            if (availableGame != null)
                return availableGame;

            return _allOtherGames.FirstOrDefault(g => g.GameType == gameType);
        }



        public ArenaFilterViewModel GetArenaFilterViewModel(string filterId)
        {
            return _availableFilters.FirstOrDefault(f => f.Label == filterId);
        }


    }
}
