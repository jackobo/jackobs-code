using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class LatestPublishXmlFile<TParent> : FileHolder<TParent>
        where TParent : IFolderHolder
    {
        public LatestPublishXmlFile(TParent parent) 
            : base(WellKnownFileName.LatestPublishXml.Name, parent)
        {
        }
    }
}
