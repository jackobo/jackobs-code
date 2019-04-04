using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper
{
    public class WpfApplicationServices : Models.IApplicationServices
    {
        public WpfApplicationServices()
        {
            System.Windows.Application.Current.Exit += Current_Exit;
        }

        void Current_Exit(object sender, System.Windows.ExitEventArgs e)
        {
            OnShuttingDown();
        }

        #region IApplicationService Members

        public event EventHandler ShuttingDown;

        private void OnShuttingDown()
        {
            var ev = ShuttingDown;
            if (ev != null)
                ev(this, EventArgs.Empty);
        }

        #endregion

        #region IApplicationService Members


        public void ShutDown()
        {
            if (System.Windows.Application.Current == null)
                return;

             System.Windows.Application.Current.Dispatcher.Invoke(new Action(() =>
                           {
                               System.Windows.Application.Current.ShutdownMode = System.Windows.ShutdownMode.OnExplicitShutdown;
                               System.Windows.Application.Current.Shutdown();
                           }));

            
            
        }

        #endregion
    }
}
