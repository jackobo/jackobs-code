using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Models.Folders;

namespace GGPInstallerBuilder.Actions
{
    public class GetLatestGGPApprovalSystemSourceCode : IInstallerBuildAction
    {
        public void Execute(IInstallerBuildContext context)
        {
            var sourceControlFolder = context.BuildConfiguration.GGPApprovalSystemSourceCodeFolder;

            context.Logger.Info($"Get Latest for {sourceControlFolder.AsString()}");

            context.SourceControlAdapter.GetLatest(sourceControlFolder);
        }
    }
}
