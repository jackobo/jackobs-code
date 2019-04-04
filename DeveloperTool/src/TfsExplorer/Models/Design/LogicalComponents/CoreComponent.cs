using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Build;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models.Design
{
    public class CoreComponent : CoreComponentBase<Folders.CoreComponentFolder>
    {
        public CoreComponent(Folders.CoreComponentFolder location, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator)
            : base(location, componentsVersionsProvider, serviceLocator)

        {
            this.ComponentUniqueId = new FileBasedComponentUniqueID(() => this.Location.ComponentUniqueIdTxt);
        }

        public CoreComponent(CoreComponentFolder location, IComponentUniqueId uniqueId, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator)
            : base(location, componentsVersionsProvider, serviceLocator)
        {
            this.ComponentUniqueId = uniqueId;
        }

        protected override IFolderWithBranchingSupport GetTargetFolder(ComponentsFolder targetComponentsFolder)
        {
            return targetComponentsFolder.Core.CoreComponent(this.Name);
        }

       
        IComponentUniqueId ComponentUniqueId { get; set; }

        protected override IComponentUniqueId GetComponentUniqueId()
        {
            return this.ComponentUniqueId;
        }

        public override IComponentRenameTransaction CreateRenameTransaction(string newName)
        {
            return new ComponentRenameTransaction<Folders.CoreComponentFolder>(this,
                                                                               this.Location,
                                                                               this.Location.Parent.CoreComponent(newName),
                                                                               this.ServiceLocator);
        }

        public override IComponentDeleteTransaction CreateDeleteTransaction()
        {
            return new ComponentDeleteTransaction<Folders.CoreComponentFolder>(this, this.Location, this.ServiceLocator, this.ComponentUniqueId);
        }

    }
}
