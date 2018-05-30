using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class PublishHistoryFolder : ChildFolderHolder<PublishHistoryFolder, IBranchFolder>
    {
        public PublishHistoryFolder(IBranchFolder parent)
            : base("PublishHistory", parent)
        {
        }

        public LatestPublishXmlFile<PublishHistoryFolder> LatestPublishXml
        {
            get { return new LatestPublishXmlFile<PublishHistoryFolder>(this); }
        }
    }
}
