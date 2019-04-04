using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public class CreateDevelopmentBranchAction : CustomContextCommand
    {
        public CreateDevelopmentBranchAction(IRootBranch logicalBranch, IServiceLocator serviceLocator)
        {
            _logicalBranch = logicalBranch;
            _serviceLocator = serviceLocator;
        }
        
        IRootBranch _logicalBranch;
        IServiceLocator _serviceLocator;

        public override string Caption
        {
            get
            {
                return $"Create DEV branch for {_logicalBranch.Version}";
            }
        }


        StandardBackgroundOperation _backgroundAction;


      

        public override void Execute(object parameter)
        {
            _backgroundAction = new StandardBackgroundOperation();
            _serviceLocator.GetInstance<IBackgroundOperationsRegion>().RegisterOperation(_backgroundAction);
            _serviceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(
                () => _logicalBranch.CreateDevelopmentBranch(ProgressHandler, ErrorHandler));
            RaiseCanExecuteChanged();
        }

        private void ErrorHandler(Exception exception)
        {
            _backgroundAction.Failed($"Failed to create DEV branch for {_logicalBranch.Version}",
                                    exception.ToString());
            

        }

        private void ProgressHandler(ProgressCallbackData eventData)
        {
            _serviceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(
                         () =>
                         {
                             RaiseCanExecuteChanged();
                             _backgroundAction.Update(eventData.Percentage, eventData.ActionDescription);
                             if (_backgroundAction.Status == BackgroundOperationStatus.Done)
                             {
                                 _serviceLocator.GetInstance<IBackgroundOperationsRegion>().UnregisterOperation(_backgroundAction);
                             }
                         }
             );
        }

        public override bool CanExecute(object parameter)
        {
            return _logicalBranch.CanCreateDevBranch();
        }

      
    }
}
