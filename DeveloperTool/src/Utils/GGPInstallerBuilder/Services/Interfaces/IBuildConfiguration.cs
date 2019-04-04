using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace GGPInstallerBuilder
{
    public interface IBuildConfiguration
    {
        
        ILocalPath InstallerProjectPath { get; }
        IServerPath GGPApprovalSystemSourceCodeFolder { get; }
        IDictionary<string, string> GlobalProperties { get; }
        Optional<ILocalPath> InstallerDeliveryFolder { get; }
    }
}
