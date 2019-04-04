using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public class CreateRootBranchAction : CustomContextCommand
    {
        public CreateRootBranchAction(IRootBranch rootBranch, IServiceLocator serviceLocator)
        {
            _rootBranch = rootBranch;
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;
        IRootBranch _rootBranch;
        public override string Caption
        {
            get
            {
                return "Branch to " + (_rootBranch.Version + 1);
            }
        }

        StandardBackgroundOperation _backgroundAction;

        public override void Execute(object parameter)
        {
            _backgroundAction = new StandardBackgroundOperation();
            _serviceLocator.GetInstance<IBackgroundOperationsRegion>().RegisterOperation(_backgroundAction);
            _serviceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(
                () =>
                {
                    try
                    {
                        _rootBranch.Branch(ProgressHandler);
                    }
                    catch(Exception ex)
                    {
                        ErrorHandler(ex);
                        throw;
                    }
                });
            RaiseCanExecuteChanged();

            
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

        private void ErrorHandler(Exception exception)
        {
            _backgroundAction.Failed($"Failed to branch from {_rootBranch.Version} to {_rootBranch.Version + 1}",
                                    exception.ToString());


        }

        public override bool CanExecute(object parameter)
        {
            return _rootBranch.CanBranch;
        }
    }
}
