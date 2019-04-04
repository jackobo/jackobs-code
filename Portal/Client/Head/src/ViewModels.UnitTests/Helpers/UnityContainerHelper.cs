using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Microsoft.Practices.Unity;
using Prism.Events;
using Rhino.Mocks;

namespace GamesPortal.Client.ViewModels.Helpers
{
    public static class UnityContainerHelper
    {
        public static IUnityContainer Create()
        {
            var container = new UnityContainer();
            container.RegisterInstance<IServiceLocator>(new UnityServiceLocator(container));
            container.RegisterInstance<IEventAggregator>(MockRepository.GenerateStub<IEventAggregator>());

            return container;

        }
    }

    

}
