using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class HotfixTriggerIni<TParent> : FileHolder<TParent>
         where TParent : IFolderHolder
    {
        public HotfixTriggerIni(TParent parent) 
            : base(WellKnownFileName.TriggerIni.Name, parent)
        {
        }
    }
}
