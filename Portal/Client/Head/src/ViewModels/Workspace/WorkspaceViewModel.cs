using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public class WorkspaceViewModel : ViewModelBase
    {
        public WorkspaceViewModel(IServiceLocator serviceLocator)
        {
            this.ServiceLocator = serviceLocator;

            this.ServiceLocator.GetInstance<IEventAggregator>().GetEvent<PubSubEvent<IExplorerBarItem>>().Subscribe(ItemSelectedHandler,  
                                                                                ThreadOption.UIThread, 
                                                                                true);
        }

        void ItemSelectedHandler(IExplorerBarItem explorerBarItem)
        {
            this.CurrentItem = explorerBarItem.CreateWorkspaceItem(this.ServiceLocator);
        }


        IServiceLocator ServiceLocator { get; set; }

        IWorkspaceItem _currentItem;

        public IWorkspaceItem CurrentItem
        {
            get { return _currentItem; }
            set
            {
                if (_currentItem != null && !object.ReferenceEquals(_currentItem, value))
                {
                    _currentItem.Dispose();
                }
                SetProperty(ref _currentItem, value);
            }
        }
    }
}
