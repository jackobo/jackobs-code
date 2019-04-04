using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;

namespace LayoutTool.ViewModels
{
    public interface IArenaGameCollectionViewModel : INotifyPropertyChanged
    {
        string Description { get; }
        bool IsNewGamesArena { get;}

        void NavigateTo(AvailableGameViewModel availableGameViewModel);
    }

    public class ArenaGameCollectionViewModel : DropableObservableCollection<ArenaGameViewModel>, IArenaGameCollectionViewModel, IDisposable
    {
        public ArenaGameCollectionViewModel(IArenaLayoutViewModel arenaLayout)
        {
            _arenaLayout = arenaLayout;
            _arenaLayout.PropertyChanged += ArenaLayout_PropertyChanged;
        }

        private void ArenaLayout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IArenaLayoutViewModel.IsNewGamesArena))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(IsNewGamesArena)));
            else if (e.PropertyName == nameof(IArenaLayoutViewModel.Description))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Description)));
        }

        IArenaLayoutViewModel _arenaLayout;

        public string Description
        {
            get { return _arenaLayout.Description; }
        }

        public bool IsNewGamesArena
        {
            get { return _arenaLayout.IsNewGamesArena; }
           
        }
        
        protected override void InsertItem(int index, ArenaGameViewModel item)
        {
            base.InsertItem(index, item);

            item.ConvertToAvailableGame().Owners.Add(this);
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];

            base.RemoveItem(index);

            RemoveOwner(removedItem);
        }

        private void RemoveOwner(ArenaGameViewModel game)
        {
            game.ConvertToAvailableGame().Owners.Remove(this);
        }


        protected override void ClearItems()
        {
            foreach (var game in this)
                RemoveOwner(game);

            base.ClearItems();
        }

        protected override bool CanDropItem(object droppedItem, DropContext context)
        {
            if (base.CanDropItem(droppedItem, context))
                return true;

            return droppedItem is AvailableGameViewModel;
        }

        protected override object DropItem(object droppedItem, DropContext context)
        {
            if (droppedItem is AvailableGameViewModel)
            {
                droppedItem = new ArenaGameViewModel(droppedItem as AvailableGameViewModel, this);
            }

            return base.DropItem(droppedItem, context);
        }

        public void Dispose()
        {
            foreach(var game in this)
            {
                RemoveOwner(game);
            }
        }

        public void NavigateTo(AvailableGameViewModel availableGameViewModel)
        {
            foreach(var game in this)
            {
                if (availableGameViewModel.Equals(game.ConvertToAvailableGame()))
                {
                    _arenaLayout.NavigateToGame(game);
                    return;
                }
            }
            
        }
    }
}
