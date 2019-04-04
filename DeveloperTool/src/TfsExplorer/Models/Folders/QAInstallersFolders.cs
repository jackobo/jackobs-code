using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Folders
{
    public class QAInstallersFolder : ChildFolderHolder<QAInstallerFolder, QAFolder>
    {
        public QAInstallersFolder(QAFolder parent)
            : base("Installers", parent)
        {
        }

        public QAInstallerFolder Installer(VersionNumber installerVersion)
        {
            return new QAInstallerFolder(installerVersion, this);
        }

        public IEnumerable<QAInstallerFolder> AllInstallers
        {
            get
            {
                if (!this.Exists())
                    return new QAInstallerFolder[0];

                return ToSourceControlFolder().GetSubfolders()
                                               .Select(folder => Installer(new VersionNumber(folder.Name)))
                                               .ToArray();
            }
        }
    }
}
