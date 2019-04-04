using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public interface IDialogShowService
    {
        void ShowOkCancelDialog(IOkCancelDialogViewModel viewModel);
    }
}
