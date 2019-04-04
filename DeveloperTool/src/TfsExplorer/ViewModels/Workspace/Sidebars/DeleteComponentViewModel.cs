using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.ViewModels.Workspace
{
    public class DeleteComponentViewModel : RenameDeleteComponentViewModel
    {
        public DeleteComponentViewModel(IMainBranch mainBranch, ILogicalComponent component, IServiceLocator serviceLocator, Action<bool> setInProgressStatus) 
            : base(mainBranch, component, serviceLocator, setInProgressStatus)
        {
        }

        protected override bool CanDoWork()
        {
            return true;
        }

        protected override void DoWork()
        {
            this.MainBranch.DeleteComponents(SimilarComponents);
        }

        protected override string GetActionDescription()
        {
            return $"Deleting {ComponentName}";
        }


        public string ComponentName
        {
            get { return this.Component.Name; }
        }
    }
}
