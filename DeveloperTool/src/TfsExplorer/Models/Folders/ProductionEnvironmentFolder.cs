using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class ProductionEnvironmentFolder : ChildFolderHolder<ProdFolder, ProdFolder>
    {
        public ProductionEnvironmentFolder(string name, ProdFolder parent) 
            : base(name, parent)
        {
        }

        public ProductionInstallersFolder Installers
        {
            get { return new ProductionInstallersFolder(this); }
        }

        public HotFixTriggerFolder<ProductionEnvironmentFolder> HotfixTrigger
        {
            get { return new HotFixTriggerFolder<ProductionEnvironmentFolder>(this); }
        }
    }

    
}
