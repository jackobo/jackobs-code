using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class CoreFolder : ChildFolderHolder<CoreFolder, ComponentsFolder>
    {
        public CoreFolder(ComponentsFolder parent) 
                    : base(WellKnownName, parent)
        {
        }

        public static readonly string WellKnownName = "Core";

        public CoreComponentFolder CoreComponent(string name)
        {
            return new CoreComponentFolder(name, this);
        }

        public IEnumerable<CoreComponentFolder> AllCoreComponents
        {
            get
            {
                if (!this.Exists())
                    return new CoreComponentFolder[0];

                return ToSourceControlFolder().GetSubfolders()
                                               .Select(folder => CoreComponent(folder.Name))
                                               .ToArray();
            }
        }

    
        public GGPBootstrapperFolder GGPBootstrapper
        {
            get { return new GGPBootstrapperFolder(this); }
        }
    }
}
