using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using Spark.Infra.Types;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels
{
    public class GamesReportFilterOptionsViewModel : ViewModelBase
    {
        public GamesReportFilterOptionsViewModel(RegulationType[] regulations, GameInfrastructure[] infrastructures)
        {
            this.RegulationsSelector = new RegulationsSelectorViewModel(regulations, true);
            this.InfrastructuresSelector = new GameInfrastructuresSelector(infrastructures, true);

            this.RegulationsSelector.PropertyChanged += RegulationsSelector_PropertyChanged;
            this.InfrastructuresSelector.PropertyChanged += RegulationsSelector_PropertyChanged;
        }

        private void RegulationsSelector_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            OnFilterChanged();
        }
        
        public GameInfrastructuresSelector InfrastructuresSelector { get; private set; }

        public RegulationsSelectorViewModel RegulationsSelector { get; private set; }

        
        private bool _isActive = false;
        public bool IsActive
        {
            get { return _isActive; }
            set
            {
                SetProperty(ref _isActive, value);
            }
        }

        private bool _onlyInternalGames = true;
        public bool OnlyInternalGames
        {
            get { return _onlyInternalGames; }
            set
            {
                if (SetProperty(ref _onlyInternalGames, value))
                {
                    SetProperty(ref _onlyExternalGames, !value, this.GetPropertyName(t => t.OnlyExternalGames));
                    SetProperty(ref _bothInternalAndExternalGames, !value, this.GetPropertyName(t => t.BothInternalAndExternalGames));
                    OnFilterChanged();
                }
            }
        }


        private bool _onlyExternalGames = false;
        public bool OnlyExternalGames
        {
            get { return _onlyExternalGames; }
            set
            {
                if (SetProperty(ref _onlyExternalGames, value))
                {
                    SetProperty(ref _onlyInternalGames, !value, this.GetPropertyName(t => t.OnlyInternalGames));
                    SetProperty(ref _bothInternalAndExternalGames, !value, this.GetPropertyName(t => t.BothInternalAndExternalGames));
                    OnFilterChanged();
                }
            }
        }


        private bool _bothInternalAndExternalGames = false;
        public bool BothInternalAndExternalGames
        {
            get { return _bothInternalAndExternalGames; }
            set
            {
                if (SetProperty(ref _bothInternalAndExternalGames, value))
                {
                    SetProperty(ref _onlyExternalGames, !value, this.GetPropertyName(t => t.OnlyExternalGames));
                    SetProperty(ref _onlyInternalGames, !value, this.GetPropertyName(t => t.OnlyInternalGames));
                    OnFilterChanged();
                }
            }
        }


        public event EventHandler FilterChanged;

        private void OnFilterChanged()
        {
            var ev = FilterChanged;
            if (ev != null)
            {
                ev(this, EventArgs.Empty);
            }
        }

    }
}
