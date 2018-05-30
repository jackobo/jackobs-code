using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
   
    public class ComponentUniqueIdTxt<TParent> : FileHolder<TParent>
        where TParent : IFolderHolder
    {
        public ComponentUniqueIdTxt(TParent parent)
            : base(WellKnownFileName.ComponentUniqueIdTxt.Name, parent)
        {
        }
        
        
    }
}
