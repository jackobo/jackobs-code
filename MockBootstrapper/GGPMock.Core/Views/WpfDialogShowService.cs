using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Views
{
    public class WpfDialogShowService : ViewModels.IDialogShowService
    {
        #region IDialogShowService Members

        public void ShowOkCancelDialog(ViewModels.IOkCancelDialogViewModel viewModel)
        {
            new OkCancelDialogView(viewModel).ShowDialog();
        }

        #endregion
    }
}
