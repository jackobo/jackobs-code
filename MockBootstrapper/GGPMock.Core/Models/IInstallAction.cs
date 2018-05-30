using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public interface IInstallAction : INotifyPropertyChanged
    {
        string Description { get; }
        void Execute(IInstalationContext context);

        int SubActionsCount { get; }
        int CurrentSubActionIndex { get; }
        string CurrentSubActionDescription { get; }
    }
}
