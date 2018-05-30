using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Views
{
    public interface IView
    {
        object ViewModel { get; set; }
    }
}
