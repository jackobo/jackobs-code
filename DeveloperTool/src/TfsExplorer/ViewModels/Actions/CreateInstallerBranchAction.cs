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
    public class CreateInstallerBranchAction : CustomContextCommand
    {
        public CreateInstallerBranchAction(IInstaller  installer, IServiceLocator serviceLocator, Action whenDoneCallback)
        {
            _installer = installer;
            _serviceLocator = serviceLocator;
            _whenDoneCallback = whenDoneCallback;
        }

        IServiceLocator _serviceLocator;
        Action _whenDoneCallback;
        IInstaller _installer;

        public override string Caption
        {
            get
            {
                return "Create branch";
            }
        }

        StandardBackgroundOperation _backgroundAction = new StandardBackgroundOperation();

        public override void Execute(object parameter)
        {
            
            _backgroundAction = new StandardBackgroundOperation();
            _serviceLocator.GetInstance<IBackgroundOperationsRegion>().RegisterOperation(_backgroundAction);
            _serviceLocator.GetInstance<IApplicationServices>().StartNewParallelTask(CreateBranch);
            RaiseCanExecuteChanged();

        }

        
        private void CreateBranch()
        {
            try
            {
                _installer.CreateBranch(ProgressHandler);
            }
            catch(Exception ex)
            {
                _backgroundAction.Failed($"Failed to create installer branch {_installer.Version}",
                                        ex.ToString());
            }
        }

        public override bool CanExecute(object parameter)
        {
            return _backgroundAction.Status == BackgroundOperationStatus.NotStarted 
                    && !_installer.IsBranched();
        }

        private void ProgressHandler(ProgressCallbackData eventData)
        {
            _serviceLocator.GetInstance<IApplicationServices>().ExecuteOnUIThread(
                         () =>
                         {
                             _backgroundAction.Update(eventData.Percentage, eventData.ActionDescription);
                             RaiseCanExecuteChanged();
                             if (_backgroundAction.Status == BackgroundOperationStatus.Done)
                             {
                                 _serviceLocator.GetInstance<IBackgroundOperationsRegion>().UnregisterOperation(_backgroundAction);
                                 _whenDoneCallback?.Invoke();
                             }
                         }
             );
        }

    }
}
