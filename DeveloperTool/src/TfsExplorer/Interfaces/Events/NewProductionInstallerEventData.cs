using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
    public class NewProductionInstallerEventData
    {
        public NewProductionInstallerEventData(IProductionInstaller installer, IProductionEnvironment environment)
        {
            this.Installer = installer;
            this.Environment = environment;
        }
        
        public IProductionInstaller Installer { get; private set; }
        public IProductionEnvironment Environment { get; private set; }
    }
}
