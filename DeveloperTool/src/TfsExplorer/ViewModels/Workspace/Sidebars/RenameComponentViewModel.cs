using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Exceptions;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class RenameComponentViewModel : RenameDeleteComponentViewModel
    {
        
        public RenameComponentViewModel(IMainBranch mainBranch, 
                                        ILogicalComponent component, 
                                        IServiceLocator serviceLocator,
                                        Action<bool> setInProgressStatus)
            : base(mainBranch, component, serviceLocator, setInProgressStatus)
        {
        }

        protected override bool CanDoWork()
        {
            return !string.IsNullOrEmpty(this.NewComponentName);
        }

        protected override void DoWork()
        {
            string newName = NewComponentName?.Trim();
            if (string.IsNullOrEmpty(newName))
            {
                throw new ValidationException("You must provide a new name for the component!");
            }

            this.MainBranch.RenameComponents(SimilarComponents, newName);
            this.ExecuteOnUIThread(this.Deactivate);
        }

      

        protected override string GetActionDescription()
        {
            return $"Renaming {OriginalComponentName} to {NewComponentName}";
        }

        public string OriginalComponentName
        {
            get { return Component.Name; }
        }


        private string _newComponentName;

        public string NewComponentName
        {
            get { return _newComponentName; }
            set { SetProperty(ref _newComponentName, value); }
        }
    }
}
