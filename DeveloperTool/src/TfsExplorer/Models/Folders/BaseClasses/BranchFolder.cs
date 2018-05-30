using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public abstract class BranchFolder<TConcreteFolder, TParent> : ChildFolderHolder<TConcreteFolder, TParent>, IBranchFolder
        where TParent : IFolderHolder
        where TConcreteFolder : IFolderHolder
    {
        public BranchFolder(string name, TParent parent)
            : base(name, parent)
        {
        }

        public ComponentsFolder Components
        {
            get { return new ComponentsFolder(this); }
        }
        
        public BuildToolsFolder BuildTools
        {
            get { return new BuildToolsFolder(this); }
        }

        public TriggerFolder Trigger
        {
            get { return new TriggerFolder(this); }
        }
        
    }
}
