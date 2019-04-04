using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
    public class NewQAInstallerEventData
    {
        public NewQAInstallerEventData(IQAInstaller installer, IQaBranch qaBranch)
        {
            this.Installer = installer;
            this.QABranch = qaBranch;
        }

        public IQAInstaller Installer { get; private set; }
        public IQaBranch QABranch { get; private set; }
    }
}
