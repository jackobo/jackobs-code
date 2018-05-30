using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;
using Spark.Wpf.Common.ViewModels;

namespace GamesPortal.Client.ViewModels.Workspace
{
    public abstract class WorkspaceItem : ServicedViewModelBase, IWorkspaceItem
    {
        public WorkspaceItem(IServiceLocator serviceLocator)
            : base(serviceLocator)
        {
        }

        
        public abstract string Title { get; }

        
    }
}
