using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Publish;

namespace Spark.TfsExplorer.Models.Design
{
    public abstract class CoreComponentBase<TLocation> : LogicalComponent<TLocation>, ICoreComponent
        where TLocation : Folders.IFolderWithBranchingSupport
    {
        public CoreComponentBase(TLocation location, Optional<IComponentsVersionsProvider> componentsVersionsProvider, IServiceLocator serviceLocator) 
            : base(location, componentsVersionsProvider, serviceLocator)
        {
            
        }

        public override string Name
        {
            get
            {
                return this.Location.Name;
            }
        }

      
        
        protected override void AcceptCommandVisitor(ILogicalComponentVisitor visitor)
        {
            visitor.Visit(this);
        }

        public override IEnumerable<IComponentPublisher> GetPublishers()
        {
            return new IComponentPublisher[] { new CoreComponentPublisher(this, this.VersionsProvider.First(), this.GetComponentUniqueId()) };
        }

        protected abstract IComponentUniqueId GetComponentUniqueId();
    }
}
