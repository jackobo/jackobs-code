using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Practices.ServiceLocation;
using Prism.Events;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Interfaces.Events;
using Spark.TfsExplorer.Models.Folders;
using Spark.TfsExplorer.Models.TFS;
using Spark.Wpf.Common.Interfaces.UI;

namespace Spark.TfsExplorer.Models.Design
{
    public class ComponentsRepository : IComponentsRepository, IRootBranchGenerator
    {
        public ComponentsRepository(IServiceLocator serviceLocator)
        {
            _serviceLocator = serviceLocator;
        }

        IServiceLocator _serviceLocator;


        ITfsGateway TfsGateway
        {
            get { return _serviceLocator.GetInstance<ITfsGateway>(); }
        }
                

        List<IRootBranch> _logicalBranches = null;
        public IEnumerable<IRootBranch> GetRootBranches()
        {

            if (_logicalBranches == null)
            {
                var logicalBranches = new List<IRootBranch>();

                foreach (var rootFolder in GetRootFolders())
                {
                    logicalBranches.Add(CreateRootBranch(rootFolder));
                }

                _logicalBranches = logicalBranches;
            }

            return _logicalBranches;
            
        }


        private IEnumerable<ISourceControlFolder> GetRootFolders()
        {
            var roots = new List<ISourceControlFolder>();
            foreach (var root in this.TfsGateway.GetRoots())
            {
                if (RootBranchVersion.TryParse(root.Name).Any())
                    roots.Add(root);
            }

            return roots;
        }

        private IRootBranch CreateRootBranch(ISourceControlFolder folder)
        {
            var rootBranchFolder = new Folders.RootBranchFolder(TfsGateway.GetRootServerPath(), folder);
            return _serviceLocator.GetInstance<ILogicalBranchComponentFactory>().CreateRootBranch(rootBranchFolder);
        }

        public IEnumerable<ISourceControlWorkItem> GetRelatedWorkItems(IEnumerable<IChangeSet> changeSet)
        {
            var changeSets = changeSet.Cast<TfsChangeSet>().Select(c => c.GetRawChangeset()).ToList();
            return TfsGateway.GetRelatedWorkItems(changeSets).Select(wi => new TfsWorkItem(wi)).ToList();
        }

        public void CreateRootBranch(IRootBranchExtended sourceBranch, 
                                     Action<ProgressCallbackData> progressCallback)
        {

            var newBranch = new RootBranchBuilder(TfsGateway, CreateRootBranch, () => _serviceLocator.GetInstance<IPublishPayloadBuilder>())
                                .Build(sourceBranch,
                                       progressCallback);

            _logicalBranches.Add(newBranch);
            _serviceLocator.GetInstance<IPubSubMediator>().Publish(new NewRootBranchEventData(newBranch));
            
            

        }
    }
    
}
