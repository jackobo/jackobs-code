using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Folders
{
    public class BuildCustomizationXml<TParent> : FileHolder<TParent>
      where TParent : IFolderHolder
    {
        public BuildCustomizationXml(TParent parent) 
            : base(WellKnownFileName.BuildCustomizationXml.Name, parent)
        {
        }
    }
}
