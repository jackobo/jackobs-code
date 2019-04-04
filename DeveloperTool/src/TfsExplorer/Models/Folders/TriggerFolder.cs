using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class TriggerFolder : ChildFolderHolder<TriggerFolder, IBranchFolder>
    {
        public TriggerFolder(IBranchFolder parent) 
            : base("Trigger", parent)
        {
        }

        public PublishXml<TriggerFolder> PublishXml
        {
            get { return new PublishXml<TriggerFolder>(this); }
        }
    }
}
