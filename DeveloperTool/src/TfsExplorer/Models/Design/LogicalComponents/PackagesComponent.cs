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
    public class PackagesComponent : CoreComponentBase<Folders.PackagesFolder>
    {
        public PackagesComponent(PackagesFolder location, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator) 
            : base(location, componentsVersionsProvider, serviceLocator)
        {
            this.ComponentUniqueId = new FileBasedComponentUniqueID(() => Location.ComponentUniqueIdTxt);
        }

        public PackagesComponent(PackagesFolder location, IComponentUniqueId uniqueId, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator)
           : base(location, componentsVersionsProvider, serviceLocator)
        {
            this.ComponentUniqueId = uniqueId;
        }

        public override IComponentDeleteTransaction CreateDeleteTransaction()
        {
            throw new NotSupportedException();
        }

        public override bool AllowRename
        {
            get
            {
                return false;
            }
        }
        
        public override bool AllowDelete
        {
            get
            {
                return false;
            }
        }
        IComponentUniqueId ComponentUniqueId { get; set; }

        protected override IFolderWithBranchingSupport GetTargetFolder(ComponentsFolder targetComponentsFolder)
        {
            return targetComponentsFolder.Packages;
        }


        public override IComponentRenameTransaction CreateRenameTransaction(string newName)
        {
            throw new NotSupportedException("Packages folder renaming is not allowed");
        }
        
        protected override IComponentUniqueId GetComponentUniqueId()
        {
            return this.ComponentUniqueId;
        }
    }
}
