using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class ProdFolder : ChildFolderHolder<ProdFolder, RootBranchFolder>
    {
        public ProdFolder(RootBranchFolder parent) 
            : base("PROD", parent)
        {
        }

        public ProductionEnvironmentFolder Environment(string name)
        {
            return new ProductionEnvironmentFolder(name, this);
        }

        public HotFixTriggerFolder<ProdFolder> HotfixTrigger
        {
            get { return new HotFixTriggerFolder<ProdFolder>(this); }
        }

        public IEnumerable<ProductionEnvironmentFolder> AllEnvironments
        {
            get
            {
                if (!this.Exists())
                    return new ProductionEnvironmentFolder[0];

                return ToSourceControlFolder().GetSubfolders()
                                               .Select(folder => Environment(folder.Name))
                                               .ToArray();
            }
        }
    }
}
