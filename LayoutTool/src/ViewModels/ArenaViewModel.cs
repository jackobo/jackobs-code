using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using LayoutTool.Interfaces.Entities;
using Microsoft.Practices.ServiceLocation;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
  
    public interface IArenaViewModel : INotifyPropertyChanged
    {
        bool IsNewGamesArena { get; }
        string Name { get; }

        void NavigateTo(ArenaLayoutViewModel arenaLayout, ArenaGameViewModel game);
    }

    public class ArenaViewModel : ServicedViewModelBase, ILobbyItemSource, IArenaViewModel
    {

        public ArenaViewModel(int type, string name, bool isNewGamesArena, IPlayerStatusFriendlyNameProvider playerStatusFriendlyNameProvider, IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
            this.Type = type;
            this.Name = name;
            this.IsNewGamesArena = isNewGamesArena;

            AddDynamicLayoutHandler = new AddDynamicLayoutHandler<ArenaLayoutViewModel>(this.Layouts,
                                                                                        playerStatus => new ArenaLayoutViewModel(this, 
                                                                                                                                playerStatus, 
                                                                                                                                GetDefaultLayout().Attributes.Clone(),
                                                                                                                                GetDefaultLayout().DataGridInfo.Clone()),
                                                                                        playerStatusFriendlyNameProvider);
            
            Layouts.CollectionChanged += Layouts_CollectionChanged;
            
        }

                
        public override string ToString()
        {
            return this.Name;
        }


        private ArenaLayoutViewModel GetDefaultLayout()
        {
            return this.Layouts.First(l => !l.PlayerStatus.IsDynamicLayout);
        }

        private void Layouts_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(LayoutsDescription));

            if(e.OldItems != null)
            {
                foreach(ArenaLayoutViewModel l in e.OldItems)
                {
                    l.PropertyChanged -= Layout_PropertyChanged;
                    l.Dispose();
                }
            }

            if(e.NewItems != null)
            {
                foreach (ArenaLayoutViewModel l in e.NewItems)
                {
                    l.PropertyChanged += Layout_PropertyChanged;
                }
            }
        }

        private void Layout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ArenaLayoutViewModel.PlayerStatus))
            {
                OnPropertyChanged(nameof(LayoutsDescription));
            }
        }

        public void NavigateTo(ArenaLayoutViewModel arenaLayout, ArenaGameViewModel game)
        {
            ServiceLocator.GetInstance<ISkinDesigner>().NavigateToWorkspace(this);
            arenaLayout.Activate(game);
        }

        public AddDynamicLayoutHandler<ArenaLayoutViewModel> AddDynamicLayoutHandler { get; private set; }

        private bool _isNewGamesArena;
        public bool IsNewGamesArena
        {
            get { return _isNewGamesArena; }
            private set
            {
                SetProperty(ref _isNewGamesArena, value);
            }
        }


        ArenaLayoutCollectionViewModel _layouts = new ArenaLayoutCollectionViewModel();
        public ArenaLayoutCollectionViewModel Layouts
        {
            get { return _layouts; }
        }
       
        public string LayoutsDescription
        {
            get
            {
                return string.Join(Environment.NewLine, Layouts.Select(l => l.PlayerStatus.ToString()));
            }
        }



        private int _type;
        public int Type
        {
            get { return _type; }
            set { SetProperty(ref _type, value); }
        }


        



        private string _name;
               

        public string Name
        {
            get { return _name; }
            set { SetProperty(ref _name, value); }
        }

        
        int ILobbyItemSource.Id
        {
            get
            {
                return this.Type;
            }
        }

        string ILobbyItemSource.Name
        {
            get
            {
                return this.Name;
            }
        }
        bool ILobbyItemSource.ShouldShowTheJackpot(PlayerStatusTypeViewModel playerStatus)
        {
            var arenaLayout = this.Layouts.FirstOrDefault(l => l.PlayerStatus.Equals(playerStatus));
            if (arenaLayout == null)
                return false;

            return arenaLayout.JackpotVisible;
        }

    }
}
