using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.ViewModels
{
    public interface IOkCancelDialogViewModel
    {
        string Title { get; }
        Action Close { get; set; }
        IActionViewModel OKAction { get; }
        IActionViewModel CancelAction { get; }
    }
}
