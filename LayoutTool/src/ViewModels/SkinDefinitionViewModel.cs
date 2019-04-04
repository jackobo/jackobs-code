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
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class SkinDefinitionViewModel : ViewModelBase
    {
        public SkinDefinitionViewModel(IServiceLocator serviceLocator)
        {
            _triggers = new TriggerViewModelCollection(serviceLocator);
            _topGames = new GameGroupLayoutCollectionViewModel("Top Games", _triggers);
            _vipGames = new GameGroupLayoutCollectionViewModel("VIP Top Games", _triggers);
        }

        
        

        private MyAccountViewModel _myAccount = new MyAccountViewModel();

        public MyAccountViewModel MyAccount
        {
            get { return _myAccount; }
            set
            {
                SetProperty(ref _myAccount, value);
            }
        }

        GameGroupLayoutCollectionViewModel _topGames;

        public GameGroupLayoutCollectionViewModel TopGames
        {
            get { return _topGames; }
            set
            {
                SetProperty(ref _topGames, value);
            }
        }

        GameGroupLayoutCollectionViewModel _vipGames;

        public GameGroupLayoutCollectionViewModel VipGames
        {
            get { return _vipGames; }
            set
            {
                SetProperty(ref _vipGames, value);
            }
        }


        public ArenaCollectionViewModel _arenas;

        public ArenaCollectionViewModel Arenas
        {
            get { return _arenas; }
            set
            {
                SetProperty(ref _arenas, value);
            }
        }


        private LobbyCollectionViewModel _lobbies;

        public LobbyCollectionViewModel Lobbies
        {
            get { return _lobbies; }
            set
            {
                SetProperty(ref _lobbies, value);
            }
        }

        private TriggerViewModelCollection _triggers;
        public TriggerViewModelCollection Triggers
        {
            get
            {
                return _triggers;
            }

            set
            {
                SetProperty(ref _triggers, value);
            }
        }

        public IEnumerable<PlayerStatusTypeViewModel> GetUsedPlayerStatus()
        {
            var playerStatuses = new List<PlayerStatusTypeViewModel>();

            playerStatuses.AddRange(ScanForDynamicLayoutPlayerStatus(Arenas.SelectMany(a => a.Layouts), layout => layout.PlayerStatus));
            playerStatuses.AddRange(ScanForDynamicLayoutPlayerStatus(Lobbies, lobbies => lobbies.PlayerStatus));
            playerStatuses.AddRange(ScanForDynamicLayoutPlayerStatus(TopGames, topGames => topGames.PlayerStatus));
            playerStatuses.AddRange(ScanForDynamicLayoutPlayerStatus(VipGames, vipGames => vipGames.PlayerStatus));
            
            return playerStatuses.Distinct().ToArray();
        }

        private IEnumerable<PlayerStatusTypeViewModel> ScanForDynamicLayoutPlayerStatus<TItem>(IEnumerable<TItem> collection, Func<TItem, PlayerStatusTypeViewModel> getPlayerStatus)
        {
            var list = new List<PlayerStatusTypeViewModel>();
            foreach (var item in collection)
            {
                var ps = getPlayerStatus(item);
                if(ps.IsDynamicLayout)
                    list.Add(ps);
            }

            return list;
        }
    }
}
