using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces;
using GamesPortal.Client.Interfaces.Services;
using Microsoft.Practices.Prism.Modularity;
using Microsoft.Practices.Unity;

namespace GamesPortal.Client.Services
{
    [Module(ModuleName = WellKnownModules.Services)]
    public class ServicesModule : IModule
    {
        public ServicesModule(IUnityContainer container)
        {
            this.Container = container;
        }

        IUnityContainer Container { get; set; }

        
        public void Initialize()
        {
            var gamesRepository = new GamesRepository();
            this.Container.RegisterInstance<IGamesRepository>(gamesRepository);
            this.Container.RegisterInstance<IReportingService>(gamesRepository);
        }

    }
}
