using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace LayoutTool.ViewModels
{
    public class ArenaFilterViewModel : DragableDropableViewModel
    {
        public ArenaFilterViewModel(string label, string name, AttributeValue[] attributes)
        {
            Label = label;
            Name = name;
            Attributes = attributes;
            _owners.CollectionChanged += Owners_CollectionChanged;
        }


        public AttributeValue[] Attributes { get; private set; }

        private void Owners_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            OnPropertyChanged(nameof(ArenasNames));

            if (e.OldItems != null)
                UnsubscribeFromOwnerPropertyChanged(e.OldItems.Cast<IArenaFiltersCollectionViewModel>());

            if (e.NewItems != null)
                SubscribeToOwnerPropertyChanged(e.NewItems.Cast<IArenaFiltersCollectionViewModel>());
        }

        public string Label { get; private set; }
        public string Name { get; private set; }


        private void SubscribeToOwnerPropertyChanged(IEnumerable<IArenaFiltersCollectionViewModel> owners)
        {
            foreach (var o in owners)
            {
                o.PropertyChanged += Owner_PropertyChanged;
            }
        }

        private void UnsubscribeFromOwnerPropertyChanged(IEnumerable<IArenaFiltersCollectionViewModel> owners)
        {
            foreach (var o in owners)
            {
                o.PropertyChanged -= Owner_PropertyChanged;
            }
        }

        private void Owner_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(IArenaFiltersCollectionViewModel.Description))
            {
                OnPropertyChanged(nameof(ArenasNames));
            }
        }

        private ObservableCollection<IArenaFiltersCollectionViewModel> _owners = new ObservableCollection<IArenaFiltersCollectionViewModel>();

        public ObservableCollection<IArenaFiltersCollectionViewModel> Owners
        {
            get { return _owners; }
        }

        public string ArenasNames
        {
            get
            {
                return string.Join(Environment.NewLine, Owners.Select(o => o.Description));
            }
        }
    }
}
