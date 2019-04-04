using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    internal class InstallerSynchronizer<TInstaller, TEventData>
        where TInstaller : IInstaller
    {
        public InstallerSynchronizer(Func<IEnumerable<TInstaller>> readInstallers,
                                     Func<TInstaller, TEventData> createEnventData,
                                     IServiceLocator serviceLocator)
        {
            _readInstallers = readInstallers;
            _createEnventData = createEnventData;
            _serviceLocator = serviceLocator;

            this.CurrentInstallers = _readInstallers();
            
            StartSyncInstallers();
        }

        public IEnumerable<TInstaller> CurrentInstallers { get; private set; }


        Func<IEnumerable<TInstaller>> _readInstallers;
        Func<TInstaller, TEventData> _createEnventData;

        private void StartSyncInstallers()
        {
            var threadingServices = _serviceLocator.GetInstance<IThreadingServices>();
            _autoResetEvent = threadingServices.CreateAutoResetEvent(false);
            threadingServices.StartNewTask(SyncInstallers);
            _serviceLocator.GetInstance<IApplicationServices>().ShuttingDown += Application_ShuttingDown;
        }

        IServiceLocator _serviceLocator;

        bool _stopSync = false;
        IAutoresetEvent _autoResetEvent;
        private void SyncInstallers()
        {
            while (!_stopSync)
            {
                _autoResetEvent.WaitOne(TimeSpan.FromSeconds(5));

                var newInstallers = _readInstallers();

                var pubSubMediator = _serviceLocator.GetInstance<IPubSubMediator>();
                foreach (var newInstaller in newInstallers.Except(CurrentInstallers))
                {
                    if (_stopSync)
                        return;

                    pubSubMediator.Publish(_createEnventData(newInstaller));
                }

                CurrentInstallers = newInstallers;
            }
        }

        private void Application_ShuttingDown(object sender, EventArgs e)
        {
            _stopSync = true;
            _autoResetEvent.Set();
        }
    }
}
