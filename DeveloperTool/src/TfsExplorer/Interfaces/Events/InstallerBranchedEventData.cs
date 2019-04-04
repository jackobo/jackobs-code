using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
    public class InstallerBranchedEventData
    {
        public InstallerBranchedEventData(IInstaller installer)
        {
            this.Installer = installer;
        }

        public IInstaller Installer { get; private set; }
    }
}
