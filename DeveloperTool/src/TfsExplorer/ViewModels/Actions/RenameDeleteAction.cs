using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public abstract class RenameDeleteAction : CustomContextCommand
    {
        public RenameDeleteAction(IComponentRenameDeleteHandler renameDeleteHandler, ILogicalComponent component)
        {
            RenameDeleteHandler = renameDeleteHandler;
            Component = component;
            RenameDeleteHandler.PropertyChanged += RenameDeleteHandler_PropertyChanged;
        }

        private void RenameDeleteHandler_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RaiseCanExecuteChanged();
        }

        protected IComponentRenameDeleteHandler RenameDeleteHandler { get; private set; }
        protected ILogicalComponent Component { get; private set; }

        public override bool CanExecute(object parameter)
        {
            return base.CanExecute(parameter) && !this.RenameDeleteHandler.InProgress;
        }


    }
}
