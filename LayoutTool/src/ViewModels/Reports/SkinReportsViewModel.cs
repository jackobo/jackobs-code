using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using LayoutTool.ViewModels.Reports;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class SkinReportsViewModel : ServicedViewModelBase
    {
        public SkinReportsViewModel(SkinDefinitionViewModel skinDefinition, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            _skinDefinition = skinDefinition;
            StartBusyAction(LoadAsync);
        }

        SkinDefinitionViewModel _skinDefinition;

        
        private void LoadAsync()
        {
            var items = new List<Reports.GamesToArenasDistributionReportItem>();


            var alsoPlaying = _skinDefinition.Arenas.SelectMany(a => a.Layouts.SelectMany(l => l.AlsoPlayingGames.Select(g => new { g.GameType, l.Description })))
                                                   .GroupBy(item => item.GameType)
                                                   .ToDictionary(g => g.Key, g => string.Join(Environment.NewLine, g.Select(x => x.Description)));

            var topGames = _skinDefinition.TopGames.SelectMany(group => group.Games.Select(g => g.GameType)).Distinct().ToDictionary(gt => gt);
            var vipTopGames = _skinDefinition.VipGames.SelectMany(group => group.Games.Select(g => g.GameType)).Distinct().ToDictionary(gt => gt);
            var lobbyGames = _skinDefinition.Lobbies.SelectMany(group => group.Items.Select(g => g.Id)).Distinct().ToDictionary(gt => gt);

            

            foreach (var arena in _skinDefinition.Arenas)
            {
                foreach (var layout in arena.Layouts)
                {
                    foreach (var game in layout.Games)
                    {
                        var g = game.ConvertToAvailableGame();
                        
                        items.Add(new Reports.GamesToArenasDistributionReportItem(g.GameGroup,
                                                                                    g.GameType,
                                                                                    g.Name,
                                                                                    g.IsNewGame,
                                                                                    g.IsVipGame,
                                                                                    alsoPlaying.ContainsKey(g.GameType) ? alsoPlaying[g.GameType] : string.Empty,
                                                                                    topGames.ContainsKey(g.GameType),
                                                                                    vipTopGames.ContainsKey(g.GameType),
                                                                                    lobbyGames.ContainsKey(g.GameType),
                                                                                    arena.Type,
                                                                                    arena.Name,
                                                                                    layout.PlayerStatus.Name));
                    }
                }
            }

            ServiceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(() => this.Items = items);
        }


        List<GamesToArenasDistributionReportItem> _items;
        public List<GamesToArenasDistributionReportItem> Items
        {
            get { return _items; }
            private set
            {
                SetProperty(ref _items, value);
            }
        }
    }
}
