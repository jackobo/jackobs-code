using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Prism.Regions;
using Spark.Wpf.Common.ViewModels;

namespace Spark.Wpf.Common.Views
{
    public abstract class StandardViewsModule : IModule
    {

        public StandardViewsModule(IUnityContainer container)
        {
            this.Container = container;
        }
        
        IUnityContainer Container { get; set; }

        void IModule.Initialize()
        {
            Initialize(Container);
        }
        

        protected virtual void Initialize(IUnityContainer container)
        {
            RegisterViewsWithViewModels(container);
            
        }

    

        protected virtual void RegisterViewsWithViewModels(IUnityContainer container)
        {
            var viewBaseUserControlClassName = nameof(StandardViewUserControl<IViewModel>);
            var userControlTypes = this.GetType().Assembly.GetTypes().Where(t => t.BaseType.Name.StartsWith(viewBaseUserControlClassName))
                                                                          .ToArray();

            foreach (var controlType in userControlTypes)
            {
                container.RegisterViewWithViewModel(controlType, controlType.BaseType.GetGenericArguments()[0]);
            }



        }
    }
}
