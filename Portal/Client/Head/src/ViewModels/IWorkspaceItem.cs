using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GamesPortal.Client.ViewModels
{
    public interface IWorkspaceItem : INotifyPropertyChanged, IDisposable
    {
        string Title { get; }
    }
}
