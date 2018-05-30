using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Prism.Regions;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;

namespace LayoutTool.ViewModels
{
    public class SkinValidator
    {
        public SkinValidator(SkinDesignerViewModel skinDesigner, IApplicationServices applicationServices)
        {
            _skinDesigner = skinDesigner;
            _applicationServices = applicationServices;
            applicationServices.StartNewParallelTask(ValidateAsync);
        }
        
        SkinDesignerViewModel _skinDesigner;
        IApplicationServices _applicationServices;
        

        private AutoResetEvent _autoResetEvent = new AutoResetEvent(true);
        private void ValidateAsync()
        {
            while(true)
            {
                var errors = new List<ErrorMessageViewModel>();

                errors.AddRange(ValidateApprovedGames());
                errors.AddRange(ValidateAlsoPlayingGames());
                errors.AddRange(ValidateLobby());
                errors.AddRange(ValidateTopGames(_skinDesigner.SkinDefinition.TopGames));
                errors.AddRange(ValidateTopGames(_skinDesigner.SkinDefinition.VipGames));
                errors.AddRange(ValidateFilters());

                errors = errors.OrderByDescending(err => err.Severity).ThenBy(err => err.SourceName).ToList();

                var errorList = new ErrorListViewModel(errors);

                _applicationServices.ExecuteOnUIThread(() =>
                {
                    _skinDesigner.ErrorList = errorList;
                }
                );

                _autoResetEvent.WaitOne();


            }
        }

        private IEnumerable<ErrorMessageViewModel> ValidateFilters()
        {
            var errors = new List<ErrorMessageViewModel>();

            foreach (var arena in _skinDesigner.SkinDefinition.Arenas)
            {
                foreach(var layout in arena.Layouts)
                {
                    if(layout.Filters.Count > Constants.MaxNumberOfFiltesInArena)
                    {
                        errors.Add(new ErrorMessageViewModel(layout.Description, 
                                                    $"The number of filters in an arena should not exceed {Constants.MaxNumberOfFiltesInArena}", 
                                                    ErrorServerity.Error,
                                                    () => NavigateToArena(arena, layout)));
                    }
                }
            }
            return errors;
        }

        private IEnumerable<ErrorMessageViewModel> ValidateTopGames(GameGroupLayoutCollectionViewModel topGames)
        {
            var errors = new List<ErrorMessageViewModel>();

            errors.AddRange(ValidateNumberOfGamesInTheTopGamesSection(topGames));
            errors.AddRange(ValidateGamesUsageInTheTopGamesSection(topGames));


            return errors;
        }

        private IEnumerable<ErrorMessageViewModel> ValidateGamesUsageInTheTopGamesSection(GameGroupLayoutCollectionViewModel topGames)
        {
            var errors = new List<ErrorMessageViewModel>();

            var allUsedGames = GetAllUsedGamesDictionary();

            foreach (var gameGroup in topGames)
            {
                foreach(var game in gameGroup.Games)
                {
                    if(!allUsedGames.ContainsKey(game.GameType))
                    {
                        errors.Add(new ErrorMessageViewModel(topGames.Title,
                                                $"The game '{game.Name}[{game.GameType}]' is used in the '{topGames.Title}' but it is not used in any arena!",
                                                ErrorServerity.Error,
                                                () => NavigateToTopGames(topGames)));
                    }
                }

            }

            return errors;

        }

        private IEnumerable<ErrorMessageViewModel> ValidateNumberOfGamesInTheTopGamesSection(GameGroupLayoutCollectionViewModel topGames)
        {
            var errors = new List<ErrorMessageViewModel>();

            foreach (var gameGroup in topGames)
            {
                if (gameGroup.Games.Count > Constants.MaxTopGamesItems)
                {
                    errors.Add(new ErrorMessageViewModel(topGames.Title,
                                                $"The number of the games in the '{topGames.Title}' must be less than or equal to {Constants.MaxTopGamesItems}",
                                                ErrorServerity.Error,
                                                () => NavigateToTopGames(topGames)));
                }
            }

            return errors;
        }


        private void NavigateToTopGames(GameGroupLayoutCollectionViewModel topGames)
        {
            _skinDesigner.NavigateToWorkspace(topGames);
        }

        private IEnumerable<ErrorMessageViewModel> ValidateApprovedGames()
        {
            var errors = new List<ErrorMessageViewModel>();

            foreach(var arena in _skinDesigner.SkinDefinition.Arenas)
            {
                foreach(var layout in arena.Layouts)
                {
                    foreach(var arenaGame in layout.Games)
                    {
                        var game = arenaGame.ConvertToAvailableGame();
                        var isApproved = game.IsApproved;
                        if (isApproved != null && isApproved.Value == false)
                        {
                            errors.Add(new ErrorMessageViewModel($"{arena.Name} arena {layout.PlayerStatus}",
                                                        $"Game '{game.Name}' [{game.GameType}] is not approved yet",
                                                        ErrorServerity.Warning,
                                                        () => NavigateToArena(arena, layout, arenaGame)));
                        }
                    }
                }
            }
            return errors;
        }


        private void NavigateToArena(ArenaViewModel arena, ArenaLayoutViewModel arenaLayout)
        {
            NavigateToArena(arena, arenaLayout, null);
        }
        private void NavigateToArena(ArenaViewModel arena, ArenaLayoutViewModel arenaLayout, ArenaGameViewModel game)
        {

            arena.NavigateTo(arenaLayout, game);
        }

        private void NavigateToLobby()
        {
            _skinDesigner.NavigateToWorkspace(_skinDesigner.SkinDefinition.Lobbies);
        }


        

        private IEnumerable<ErrorMessageViewModel> ValidateLobby()
        {
            var errors = new List<ErrorMessageViewModel>();
            foreach(var lobby in _skinDesigner.SkinDefinition.Lobbies)
            {
                if(_skinDesigner.SkinDefinition.Lobbies.AlowedNumberOfItems != lobby.Items.Count)
                {
                    errors.Add(new ErrorMessageViewModel($"Lobby {lobby.PlayerStatus}", 
                                                $"Number of items in the lobby must be equals to {_skinDesigner.SkinDefinition.Lobbies.AlowedNumberOfItems}", 
                                                ErrorServerity.Error, 
                                                () => NavigateToLobby()));
                }
            }

            return errors;
        }

        private IEnumerable<ErrorMessageViewModel> ValidateAlsoPlayingGames()
        {
            var errors = new List<ErrorMessageViewModel>();

            errors.AddRange(ValidateNumberOfAlsoPlayingGames());
            errors.AddRange(ValidateGamesUsageInTheAlsoPlayingSection());
            return errors;
        }

        private IEnumerable<ErrorMessageViewModel> ValidateGamesUsageInTheAlsoPlayingSection()
        {
            var allUsedGames = GetAllUsedGamesDictionary();

            var errors = new List<ErrorMessageViewModel>();
            foreach (var arena in _skinDesigner.SkinDefinition.Arenas)
            {
                foreach (var layout in arena.Layouts)
                {
                    foreach(var game in layout.AlsoPlayingGames)
                    {
                        if(!allUsedGames.ContainsKey(game.GameType))
                        {
                            errors.Add(new ErrorMessageViewModel($"{arena.Name} arena {layout.PlayerStatus}",
                                 $"The game '{game.Name} [{game.GameType}]' is used in the also playing section but is not used in any arena",
                                 ErrorServerity.Error,
                                 () => NavigateToArena(arena, layout)));
                        }
                    }
                }
            }
            return errors;

        }

        private Dictionary<int, AvailableGameViewModel> GetAllUsedGamesDictionary()
        {
            return _skinDesigner.GetAllUsedGames().ToDictionary(game => game.GameType);
        }

    

        private IEnumerable<ErrorMessageViewModel> ValidateNumberOfAlsoPlayingGames()
        {
            var errors = new List<ErrorMessageViewModel>(); 
            foreach (var arena in _skinDesigner.SkinDefinition.Arenas)
            {
                foreach (var layout in arena.Layouts)
                {
                    if (layout.AlsoPlayingGames.Count > 2)
                    {
                        errors.Add(new ErrorMessageViewModel($"{arena.Name} arena {layout.PlayerStatus}",
                                  "You cannot have more than two games in the Also Playing section",
                                  ErrorServerity.Error,
                                  () => NavigateToArena(arena, layout)));
                    }
                }
            }
            return errors;
        }
        

        public void Validate()
        {
            _autoResetEvent.Set();
        }
    }
}
