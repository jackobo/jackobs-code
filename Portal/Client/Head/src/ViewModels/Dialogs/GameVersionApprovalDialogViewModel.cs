using System;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;
using GGPGameServer.ApprovalSystem.Common;
using Prism.Commands;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Dialogs
{
    public class GameVersionApprovalDialog : ViewModelBase, IOkCancelDialogBoxViewModel
    {
        public GameVersionApprovalDialog(string[] availableStates, ClientType[] clientTypes)
        {
            this.AvailableStates = availableStates;
            this.ClientTypesSelector = new ClientTypesSelectorViewModel(clientTypes);
            
        }

        public Action CustomOkAction { get; set; }


        public ClientTypesSelectorViewModel ClientTypesSelector { get; private set; }

        public ClientType[] GetSelectedClientTypes()
        {
            return ClientTypesSelector.GetSelectedClientTypes();
        }

        string _title;

        public string Title
        {
            get { return _title; }
            set
            {
                _title = value;
                OnPropertyChanged(() => Title);
            }
        }

        string _availableStatesLabel;

        public string AvailableStatesLabel
        {
            get { return _availableStatesLabel; }
            set
            {
                _availableStatesLabel = value;
                OnPropertyChanged(() => AvailableStatesLabel);
            }
        }
        
        public string[] AvailableStates { get; private set; }


        string _selectedState;

        public string SelectedState
        {
            get { return _selectedState; }
            set
            {
                _selectedState = value;
                OnPropertyChanged(() => SelectedState);
            }
        }

        public void ExecuteOk()
        {
            if (string.IsNullOrEmpty(this.SelectedState))
            {
                throw new ValidationException(string.Format("You must select a value for '{0}' field!", this.AvailableStatesLabel));
            }

            if (this.GetSelectedClientTypes().Length == 0)
            {
                throw new ValidationException("You must select at least one regulation for one client type!");
            }

            if (CustomOkAction != null)
                CustomOkAction();
        }

        public void ExecuteCancel()
        {

        }

        public ICommand ViewByClientTypeCommand
        {
            get { return ClientTypesSelector.ViewByClientTypeCommand; }
        }

        public ICommand ViewByRegulationCommand
        {
            get { return ClientTypesSelector.ViewByRegulationCommand; }
        }


        public ClientTypesSelectorViewModel.ViewByStrategyBase CurrentViewMode
        {
            get { return ClientTypesSelector.CurrentViewMode; }
        }

        public ClientTypesSelectorViewModel.ViewByClientTypeStrategy ViewByClientType
        {
            get { return ClientTypesSelector.ViewByClientType; }
        }

        public ClientTypesSelectorViewModel.ViewByRegulationStrategy ViewByRegulation
        {
            get { return ClientTypesSelector.ViewByRegulation; }
        }
    

        

       
    }
}
