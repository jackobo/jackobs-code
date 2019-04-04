using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces;
using GamesPortal.Client.Interfaces.Services;
using GamesPortal.Client.Models.Services;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace GamesPortal.Client.Models
{
    [Module(ModuleName = WellKnownModules.Models)]
    public class ModelsModule : IModule
    {
        public ModelsModule(IUnityContainer container)
        {
            this.Container = container;
        }

        IUnityContainer Container { get; set; }


        public void Initialize()
        {

            var gamesRepository = this.Container.Resolve<GamesRepository>();
            this.Container.RegisterInstance<IGamesRepository>(gamesRepository);
            this.Container.RegisterInstance<IReportingService>(gamesRepository);
            this.Container.RegisterInstance<IGamesRepositorySynchronizer>(gamesRepository);
            
            this.Container.RegisterType<IDownloadManager, DownloadManager>();

            
            this.Container.RegisterType<IZipFileExtractor, ZipFileExtractor>();
            
            
            this.Container.RegisterInstance<SignalRHandler>(this.Container.Resolve<SignalRHandler>());
            
        }



    }
}
