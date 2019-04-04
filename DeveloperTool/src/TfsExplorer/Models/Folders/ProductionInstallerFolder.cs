using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Folders
{
    public class ProductionInstallerFolder : BranchFolder<ProductionInstallerFolder, ProductionInstallersFolder>, 
                                            IInstallerFolder
    {
        public ProductionInstallerFolder(VersionNumber installerVersion, ProductionInstallersFolder parent)
            : base(installerVersion.ToString(), parent)
        {
            
        }

        public PublishHistoryFolder PublishHistory
        {
            get
            {
                return new PublishHistoryFolder(this);
            }
        }
    }
}
