using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.TfsExplorer.ViewModels.Workspace;
using Spark.Wpf.Common.Interfaces.UI;
using Spark.Wpf.Common.ViewModels;

namespace Spark.TfsExplorer.ViewModels.Actions
{
    public class PublishToQAAction : CustomContextCommand
    {

        public PublishToQAAction(IBranchPublisherViewModel branchPublisher,
                                IServiceLocator serviceLocator,
                                string caption = "Publish")
        {
            _branchPublisher = branchPublisher;
            _serviceLocator = serviceLocator;
            _caption = caption;
        }

        IBranchPublisherViewModel _branchPublisher;
        IServiceLocator _serviceLocator;

        private string _caption;
        public override string Caption
        {
            get
            {
                return _caption;
            }
        }

        public override void Execute(object parameter)
        {
            if(_branchPublisher.IsPublishInProgress)
            {
                _serviceLocator.GetInstance<IMessageBox>().ShowMessage($"You can't publish yet because there is a publish in progress!{Environment.NewLine}Let this build finish and then you can publish again.");
                return;
            }

            _branchPublisher.StartPublishing();
        }
    }
}
