using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public class WorkAreaItemBase : ViewModelBase, IWorkAreaItemViewModel
    {
        public WorkAreaItemBase(IWorkArea workArea)
        {
            this.WorkArea = workArea;
        }

        public IWorkArea WorkArea { get; private set; }
    }
}
