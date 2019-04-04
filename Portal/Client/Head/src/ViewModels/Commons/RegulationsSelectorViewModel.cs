using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels
{
    public class RegulationsSelectorViewModel : ViewModelBase
    {
        public RegulationsSelectorViewModel(RegulationType[] regulations, bool defaultSelected)
        {
            this.Regulations = regulations.Distinct().Select(r => new ItemSelector<RegulationType>(r, defaultSelected)).ToArray();
            SubscribeToItemsPropertyChanged();
            this.SelectAllCommand = new Command(() => SelectAll(true));
            this.UnselectAllCommand = new Command(() => SelectAll(false));
            
        }

        private bool _isReadOnly = false;

        public bool IsReadOnly
        {
            get { return _isReadOnly; }
            set { SetProperty(ref _isReadOnly, value); }
        }



        private void SelectAll(bool isSelected)
        {
            UnsubscribeFromItemsPropertyChanged();
            try
            {
                foreach (var r in this.Regulations)
                    r.Selected = isSelected;

                OnPropertyChanged(nameof(SelectedRegulations));
            }
            finally
            {
                SubscribeToItemsPropertyChanged();
            }
            
        }

        private void SubscribeToItemsPropertyChanged()
        {
            foreach (var r in this.Regulations)
                r.PropertyChanged += R_PropertyChanged;
        }

        private void UnsubscribeFromItemsPropertyChanged()
        {
            foreach (var r in this.Regulations)
                r.PropertyChanged -= R_PropertyChanged;
        }

        public ICommand SelectAllCommand { get; private set; }
        public ICommand UnselectAllCommand { get; private set; }

        private void R_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if(e.PropertyName == nameof(ItemSelector<RegulationType>.Selected))
            {
                OnPropertyChanged(nameof(SelectedRegulations));
            }
        }

        public ItemSelector<RegulationType>[] Regulations { get; private set; }

        public RegulationType[] SelectedRegulations
        {
            get
            {
                return this.Regulations.Where(r => r.Selected).Select(r => r.Item).ToArray();
            }
        }
    }
}
