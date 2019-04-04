using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class PublishXml<TParent> : FileHolder<TParent>
        where TParent : IFolderHolder
    {
        public PublishXml(TParent parent) 
            : base(WellKnownFileName.PublishXml.Name, parent)
        {
        }

       
    }
}
