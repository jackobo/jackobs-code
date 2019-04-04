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
    public class GameInfrastructuresSelector : ViewModelBase
    {
        public GameInfrastructuresSelector(GameInfrastructure[] infrastructures, bool defaultSelected)
        {
            this.Infrastructures = infrastructures.Distinct().OrderBy(i => i.GameTechnology).ThenBy(i => i.PlatformType).Select(r => new ItemSelector<GameInfrastructure>(r, defaultSelected)).ToArray();
            SubscribeToItemsPropertyChanged();
            this.SelectAllCommand = new Command(() => SelectAll(true));
            this.UnselectAllCommand = new Command(() => SelectAll(false));

        }

        private void SelectAll(bool isSelected)
        {
            UnsubscribeFromItemsPropertyChanged();
            try
            {
                foreach (var r in this.Infrastructures)
                    r.Selected = isSelected;

                OnPropertyChanged(nameof(SelectedInfrastructures));
            }
            finally
            {
                SubscribeToItemsPropertyChanged();
            }

        }

        private void SubscribeToItemsPropertyChanged()
        {
            foreach (var r in this.Infrastructures)
                r.PropertyChanged += R_PropertyChanged;
        }

        private void UnsubscribeFromItemsPropertyChanged()
        {
            foreach (var r in this.Infrastructures)
                r.PropertyChanged -= R_PropertyChanged;
        }

        public ICommand SelectAllCommand { get; private set; }
        public ICommand UnselectAllCommand { get; private set; }

        private void R_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ItemSelector<RegulationType>.Selected))
            {
                OnPropertyChanged(nameof(SelectedInfrastructures));
            }
        }

        public ItemSelector<GameInfrastructure>[] Infrastructures { get; private set; }

        public GameInfrastructure[] SelectedInfrastructures
        {
            get
            {
                return this.Infrastructures.Where(r => r.Selected).Select(r => r.Item).ToArray();
            }
        }
    }
}
