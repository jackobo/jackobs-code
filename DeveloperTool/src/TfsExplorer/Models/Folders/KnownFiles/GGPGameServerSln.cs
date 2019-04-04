using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class GGPGameServerSln<TParent> : FileHolder<TParent>
      where TParent : IFolderHolder
    {
        public GGPGameServerSln(TParent parent)
            : base(WellKnownFileName.GGPGameServerSln.Name, parent)
        {
        }
    }
}
