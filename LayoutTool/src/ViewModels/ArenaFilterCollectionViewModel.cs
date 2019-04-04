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
    public interface IArenaFiltersCollectionViewModel : INotifyPropertyChanged
    {
        string Description { get; }
    }

    public class ArenaFilterCollectionViewModel : DropableObservableCollection<ArenaFilterViewModel>, IArenaFiltersCollectionViewModel, IDisposable
    {
        public ArenaFilterCollectionViewModel(IArenaLayoutViewModel arenaLayout)
        {
            _arenaLayout = arenaLayout;
            _arenaLayout.PropertyChanged += _arenaLayout_PropertyChanged;
        }

        private void _arenaLayout_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IArenaLayoutViewModel.Description))
                OnPropertyChanged(new PropertyChangedEventArgs(nameof(Description)));
        }

        IArenaLayoutViewModel _arenaLayout;
        public string Description
        {
            get { return _arenaLayout.Description; }
        }
        

        protected override void InsertItem(int index, ArenaFilterViewModel item)
        {
            base.InsertItem(index, item);

            item.Owners.Add(this);
        }

        protected override void RemoveItem(int index)
        {
            var removedItem = this[index];

            base.RemoveItem(index);

            RemoveOwner(removedItem);
        }

        private void RemoveOwner(ArenaFilterViewModel filter)
        {
            filter.Owners.Remove(this);
        }

        protected override void ClearItems()
        {
            foreach(var item in this)
            {
                RemoveOwner(item);
            }
            base.ClearItems();
        }

        public void Dispose()
        {
            foreach(var filter in this)
            {
                RemoveOwner(filter);
            }
        }
    }
}
