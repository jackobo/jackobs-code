using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LayoutTool.Interfaces;
using Microsoft.Practices.Unity;
using Prism.Modularity;

namespace LayoutTool.ViewModels
{
    [Module(ModuleName = WellKnownModules.ViewModels)]
    [ModuleDependency(WellKnownModules.Models)]
    public class ViewModelsModule : IModule
    {
        public ViewModelsModule(IUnityContainer container )
        {
            this.Container = container;
        }

        IUnityContainer Container { get; set; }
        public void Initialize()
        {
            this.Container.RegisterInstance<IToolBox>(new ToolBoxViewModel());
            
            RegisterSkinSelector();
            RegisterSkinToDesignSelectorSingleton();

        }

        private void RegisterSkinToDesignSelectorSingleton()
        {

            this.Container.RegisterInstance(Container.Resolve<SkinToDesignSelectorViewModel>());

        }

        private void RegisterSkinSelector()
        {
            var skinReadersTypes = this.GetType().Assembly.GetTypes().Where(t => !t.IsAbstract && typeof(ISkinDefinitionBuilderViewModel).IsAssignableFrom(t))
                                    .ToArray();


            foreach(var readerType in skinReadersTypes)
            {
                this.Container.RegisterType(typeof(ISkinDefinitionBuilderViewModel), readerType, readerType.FullName);
            }
            
        }
    }
}
