using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Design
{
    internal class ComponentsRenameDeleteExecutor
    {
        public ComponentsRenameDeleteExecutor(TFS.ITfsGateway tfsGateway)
        {
            _tfsGateway = tfsGateway;
        }
        
        TFS.ITfsGateway _tfsGateway;

        public void DeleteComponents(IEnumerable<ILogicalComponent> components, RootBranchVersion branchName)
        {
            if (!components.Any())
            {
                throw new ArgumentException("No components to delete provided!");
            }
            
            var componentsUniqueIDs = DeleteAndGetComponentsUniqueIDs(components).ToArray();
            
            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                proxy.DeleteComponents(new DeveloperToolService.DeleteComponentsRequest()
                {
                    Branch = branchName.ToString(),
                    ComponentsUniqueIDs = componentsUniqueIDs
                });
            }

        }

        string JoinComponentsNames(IEnumerable<ILogicalComponent> components)
        {
            return string.Join(",", components.Select(c => c.Name).Distinct());
        }

        private IEnumerable<string> DeleteAndGetComponentsUniqueIDs(IEnumerable<ILogicalComponent> sameComponents)
        {
            var uniqueIds = new List<string>();
            var deleteTransactions = sameComponents.Select(c => c.CreateDeleteTransaction()).ToList();
            using (var checkInTransaction = _tfsGateway.CreateCheckInTransaction())
            {
                foreach (var t in deleteTransactions)
                {
                    uniqueIds.AddRange(t.GetUniqueIDs()
                                      .Select(id => id.Value));
                    checkInTransaction.PendDelete(t.LocalPath);
                }

                var checkInMessage = $"Delete component(s) {JoinComponentsNames(sameComponents)}";

                checkInTransaction.CheckIn(checkInMessage, checkInMessage);

                foreach (var t in deleteTransactions)
                {
                    t.Commit();
                }
            }

            return uniqueIds;
        }
        

        public void RenameComponents(IEnumerable<ILogicalComponent> sameComponents, string newName)
        {
            if (!sameComponents.Any())
            {
                throw new ArgumentException("No components to rename provided!");
            }

            CheckComponentsAreTheSame(sameComponents);

            using (var checkInTransaction = _tfsGateway.CreateCheckInTransaction())
            {
                var renameTransactions = sameComponents.Select(c => c.CreateRenameTransaction(newName)).ToList();

                foreach (var t in renameTransactions)
                {
                    checkInTransaction.PendRename(t.OldServerPath, t.NewServerPath);
                }

                var checkInMessage = $"Rename component {sameComponents.First().Name} to {newName}";
                checkInTransaction.CheckIn(checkInMessage, checkInMessage);

                foreach (var t in renameTransactions)
                {
                    t.Commit();
                }
            }
        }

        private void CheckComponentsAreTheSame(IEnumerable<ILogicalComponent> sameComponents)
        {
            var firstComponent = sameComponents.First();

            foreach (var c in sameComponents.Skip(1))
            {
                if (!firstComponent.SameAs(c))
                {
                    throw new ArgumentException($"All renamed components should refer to the same logical component! Component {firstComponent.Name} is completelly different component than {c.Name}");
                }
            }
        }
    }
}
