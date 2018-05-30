using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class HotFixTriggerFolder<TParent> : ChildFolderHolder<HotFixTriggerFolder<TParent>, TParent>
        where TParent : IFolderHolder
    {
        public HotFixTriggerFolder(TParent parent)
            : base("HotfixTrigger", parent)
        {
        }

        public HotfixTriggerIni<HotFixTriggerFolder<TParent>> TriggerIni
        {
            get { return new HotfixTriggerIni<HotFixTriggerFolder<TParent>>(this); }
        }
    }
}
