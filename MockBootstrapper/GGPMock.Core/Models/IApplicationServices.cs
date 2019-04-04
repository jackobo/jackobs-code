using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public interface IApplicationServices
    {
        event EventHandler ShuttingDown;
        void ShutDown();
    }
}
