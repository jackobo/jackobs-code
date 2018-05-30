using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.Publish;

namespace Spark.TfsExplorer.Models.Design
{
    public class NonDeployableComponents : LogicalComponent<Folders.NonDeployableFolder>, ICoreComponent
    {
        public NonDeployableComponents(NonDeployableFolder location, IServiceLocator serviceLocator) 
            : base(location, Optional<IComponentsVersionsProvider>.None(), serviceLocator)
        {
        }

        public override string Name
        {
            get
            {
                return this.Location.Name;
            }
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

        public override IComponentDeleteTransaction CreateDeleteTransaction()
        {
            throw new NotSupportedException();
        }

        public override IComponentRenameTransaction CreateRenameTransaction(string newName)
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<IComponentPublisher> GetPublishers()
        {
            return new IComponentPublisher[0];
        }

        protected override void AcceptCommandVisitor(ILogicalComponentVisitor visitor)
        {
            visitor.Visit(this);
        }

        protected override IFolderWithBranchingSupport GetTargetFolder(ComponentsFolder targetComponentsFolder)
        {
            return targetComponentsFolder.NonDeployable;
        }
    }
}
