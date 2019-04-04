using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using GamesPortal.Client.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace GamesPortal.Client.ViewModels
{
    [Module(ModuleName = WellKnownModules.ViewModels)]
    [ModuleDependency(WellKnownModules.Models)]
    public class ViewModelsModule : IModule
    {
        public ViewModelsModule(IUnityContainer container)
        {
            this.Container = container;
        }

        IUnityContainer Container { get; set; }
        

        public void Initialize()
        {
            this.Container.RegisterInstance<NotificationArea.INotificationArea>(this.Container.Resolve<NotificationArea.NotificationAreaViewModel>());
        }

        
    }
}
