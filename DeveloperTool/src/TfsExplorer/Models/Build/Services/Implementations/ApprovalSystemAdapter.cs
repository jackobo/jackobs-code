using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public class ApprovalSystemAdapter : IApprovalSystemAdapter
    {
        public void BeginBuild(GGPDeploymentContent buildContentInfo)
        {
            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                this.TransactionID = proxy.BeginBuild(buildContentInfo.CreateBeginBuildRequest()).TransactionID;
            }
        }

        Guid? TransactionID { get; set; }

        public void EndBuild(int changeSetID)
        {
            if (TransactionID == null)
                throw new InvalidOperationException("You can't call End before calling the Begin");

            using (var proxy = DeveloperToolServiceProxyFactory.CreateProxy())
            {
                proxy.EndBuild(new DeveloperToolService.EndBuildRequest()
                {
                    TransactionID = this.TransactionID.Value,
                    ChangeSetId = changeSetID
                });
            }
        }
    }
}
