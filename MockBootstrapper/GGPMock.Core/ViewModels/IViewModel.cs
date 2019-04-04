using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public interface IViewModel : INotifyPropertyChanged
    {
        GGPGameServer.ApprovalSystem.Common.IUserInterfaceServices UIServices { get; }
    }
}
