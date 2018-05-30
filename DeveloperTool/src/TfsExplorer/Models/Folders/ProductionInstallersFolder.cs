using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Folders
{
    public class ProductionInstallersFolder : ChildFolderHolder<ProductionInstallersFolder, ProductionEnvironmentFolder>
    {
        public ProductionInstallersFolder(ProductionEnvironmentFolder parent) 
            : base("Installers", parent)
        {
        }

        public ProductionInstallerFolder Installer(VersionNumber installerVersion)
        {
            return new ProductionInstallerFolder(installerVersion, this);
        }

        public IEnumerable<ProductionInstallerFolder> AllInstallers
        {
            get
            {
                if (!this.Exists())
                    return new ProductionInstallerFolder[0];

                return ToSourceControlFolder().GetSubfolders()
                                               .Select(folder => Installer(new VersionNumber(folder.Name)))
                                               .ToArray();
            }
        }
    }
}
