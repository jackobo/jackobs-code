using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using Microsoft.Practices.Unity;

namespace GamesPortal.Service.Services.WCF
{
    /// <summary>
    /// This service host is used to set up the service behavior that replaces the instance provider to use dependency injection.
    /// </summary>
    public class DependencyInjectionServiceHost<TServiceType> : ServiceHost
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DependencyInjectionServiceHost"/> class.
        /// </summary>
        /// <param name="serviceType">Type of the service.</param>
        /// <param name="baseAddresses">The base addresses.</param>
        public DependencyInjectionServiceHost(IUnityContainer container, params Uri[] baseAddresses)
            : base(typeof(TServiceType), baseAddresses)
        {
            this.Container = container;
        }

        IUnityContainer Container { get; set; }

        /// <summary>
        /// Opens the channel dispatchers.
        /// </summary>
        /// <param name="timeout">The <see cref="T:System.Timespan"/> that specifies how long the on-open operation has to complete before timing out.</param>
        protected override void OnOpen(TimeSpan timeout)
        {
            Description.Behaviors.Add(new DependencyInjectionServiceBehavior(this.Container));
            base.OnOpen(timeout);
        }
    }
}
