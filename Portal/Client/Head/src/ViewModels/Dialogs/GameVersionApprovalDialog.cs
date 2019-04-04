using System;
using System.Linq;
using System.Windows.Input;
using GamesPortal.Client.Interfaces.Entities;

using Prism.Commands;
using Spark.Infra.Exceptions;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Dialogs
{
    public class GameVersionApprovalDialog : ViewModelBase, IOkCancelDialogBoxViewModel
    {
        public GameVersionApprovalDialog(RegulationType[] regulations)
        {
            this.RegulationsSelector = new RegulationsSelectorViewModel(regulations, true);
        }

        
        public bool IsReadOnly
        {
            get { return this.RegulationsSelector.IsReadOnly; }
            set
            {
                this.RegulationsSelector.IsReadOnly = value;
                OnPropertyChanged();
            }
        }

        public Action CustomOkAction { get; set; }

        
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

        public RegulationsSelectorViewModel RegulationsSelector { get; private set; }
        

        public void ExecuteOk()
        {
            if (RegulationsSelector.SelectedRegulations.Length == 0)
            {
                throw new ValidationException("You must select at least one regulation");
            }
            
            CustomOkAction?.Invoke();
        }

        public void ExecuteCancel()
        {

        }       
    }
}
