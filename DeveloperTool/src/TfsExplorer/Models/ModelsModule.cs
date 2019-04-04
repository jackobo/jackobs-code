using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.Unity;
using Prism.Modularity;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Publish;
using Spark.Wpf.Common;

namespace Spark.TfsExplorer.Models
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
            this.Container.RegisterInstance<IWorkspaceSelector>(new TFS.AutomaticWorkspaceSelector(TFS.TfsGateway.ROOT_FOLDER));
            this.Container.RegisterInstance<TFS.ITfsGateway>(Container.Resolve<TFS.TfsGateway>());

            var componentsRepository = Container.Resolve<Design.ComponentsRepository>();
            this.Container.RegisterInstance<IComponentsRepository>(componentsRepository);
            this.Container.RegisterInstance<Design.IRootBranchGenerator>(componentsRepository);

            this.Container.RegisterType<Design.IFeaturesBranchesReader, Design.FeaturesBranchesReader>();
            this.Container.RegisterType<Design.IMainDevBranchBuilder, Design.MainDevBranchBuilder>();
            this.Container.RegisterType<Design.IComponentsReaderFactory, Design.ComponentsReaderFactory>();
            this.Container.RegisterType<Design.ILogicalBranchComponentFactory, Design.LogicalBranchComponentFactory>();
            this.Container.RegisterType<IPublishPayloadSerializer, PublishPayloadXmlSerializer>();
            this.Container.RegisterType<IPublishPayloadBuilder, PublishPayloadBuilder>();
            this.Container.RegisterType<IComponentsPublishersReader, ComponentsPublishersReader>();
            this.Container.RegisterType<Design.IMergeSetsReader, Design.MergeSetsReader>();
        }
    }
}
