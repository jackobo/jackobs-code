using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Folders
{
    public class QAInstallerFolder : ChildFolderHolder<QAInstallerFolder, QAInstallersFolder>, IInstallerFolder
    {
        public QAInstallerFolder(VersionNumber installerVersion, QAInstallersFolder parent) 
            : base(installerVersion.ToString(), parent)
        {
        }

        public BuildToolsFolder BuildTools
        {
            get
            {
                return new BuildToolsFolder(this);
            }
        }

        public ComponentsFolder Components
        {
            get
            {
                return new ComponentsFolder(this);
            }
        }

        public PublishHistoryFolder PublishHistory
        {
            get
            {
                return new PublishHistoryFolder(this);
            }
        }

        public TriggerFolder Trigger
        {
            get
            {
                return new TriggerFolder(this);
            }
        }
    }
}
