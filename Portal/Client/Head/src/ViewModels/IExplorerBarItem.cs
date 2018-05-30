using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Microsoft.Practices.ServiceLocation;

namespace GamesPortal.Client.ViewModels
{
    public interface IExplorerBarItem : INotifyPropertyChanged
    {
        string Caption { get; }
        IWorkspaceItem CreateWorkspaceItem(IServiceLocator serviceLocator);

    }
}
