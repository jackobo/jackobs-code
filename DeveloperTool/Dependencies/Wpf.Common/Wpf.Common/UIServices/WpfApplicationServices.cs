using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Prism.Logging;
using Spark.Infra.DependencyInjection;
using Spark.Infra.Logging;
using Spark.Infra.Windows;
using Spark.Wpf.Common.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.Wpf.Common.UIServices
{
    public class WpfApplicationServices : IApplicationServices, IDependencyInjectionAware
    {
        string _applicationUniqueName;

        ILogger _logger;
        IThreadingServices _threadingServices;

        public WpfApplicationServices(string applicationUniqueName, ILoggerFactory loggerFactory, IThreadingServices threadingServices)
        {
            _applicationUniqueName = applicationUniqueName;
            _logger = loggerFactory.CreateLogger(this.GetType());
            _threadingServices = threadingServices;

            _userInterfaceServices = new WpfUserInterfaceServices(this);
          
            Application.Current.Exit += Application_Exit;
            threadingServices.StartNewTask(RegisterToMainWindowClosing);
                       
        }


        public void RegisterWithContainer(IDependencyInjectionContainer container)
        {
            container.RegisterInstance<IApplicationServices>(this);
            container.RegisterInstance(this.UserInterfaceServices);
            _userInterfaceServices.RegisterWithContainer(container);
        }


        WpfUserInterfaceServices _userInterfaceServices;
        public IUserInterfaceServices UserInterfaceServices
        {
            get { return _userInterfaceServices; }
        }


        public Task StartNewParallelTask(Action action)
        {
            if (action == null)
                throw new ArgumentNullException(nameof(action));


            return _threadingServices.StartNewTask(action,
                ex =>
                {
                    this.UserInterfaceServices.MessageBox.ShowMessage(ex.Message.Substring(0, Math.Min(1000, ex.Message.Length)));   
                });
        }

        private void RegisterToMainWindowClosing()
        {
         
            Window mainWindow = null;

            do
            {
                Application.Current.Dispatcher.Invoke(() => mainWindow = Application.Current.MainWindow);
                System.Threading.Thread.Sleep(100);
            } while (mainWindow == null);

            mainWindow.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            foreach (var exitHandler in _applicationExitHandlers)
            {
                try
                {
                    if (!exitHandler.CanExit())
                    {
                        e.Cancel = true;
                        return;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Exception($"{nameof(WpfApplicationServices)}.{nameof(MainWindow_Closing)} failed for one of the handlers! Handler is {exitHandler.ToString()}!",
                                      ex);

                                     
                }
            }
        }

        public event EventHandler ShuttingDown;

        private void Application_Exit(object sender, ExitEventArgs e)
        {
            var ev = ShuttingDown;
            if(ev != null)
            {
                foreach(EventHandler listener in ev.GetInvocationList())
                {
                    if (listener == null)
                        continue;

                    try
                    {
                        listener(this, EventArgs.Empty);
                    }
                    catch(Exception ex)
                    {
                        _logger.Exception($"{nameof(WpfApplicationServices)}.{nameof(Application_Exit)} failed for one of the listeners! Listener is {listener.ToString()}!",
                                          ex);
                    }

                }
                
            }
        }

        public void ExecuteOnUIThread(Action action)
        {
            Application.Current.Dispatcher.Invoke(action);
            
        }

        #region IApplicationServices Members


        public void Exit()
        {
            Application.Current.Shutdown();
        }

        public string GetApplicationUniqueName()
        {
            return _applicationUniqueName;
        }

        public string GetUserAppDataFolder()
        {
            return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), GetApplicationUniqueName());
        }

        private List<IApplicationExitHandler> _applicationExitHandlers = new List<IApplicationExitHandler>();

        public void RegisterApplicationExitHandler(IApplicationExitHandler handler)
        {
            _applicationExitHandlers.Add(handler);
           


        }

        public void UnregisterApplicationExitHandler(IApplicationExitHandler handler)
        {
            _applicationExitHandlers.Remove(handler);
        }

      

        #endregion
    }
}
