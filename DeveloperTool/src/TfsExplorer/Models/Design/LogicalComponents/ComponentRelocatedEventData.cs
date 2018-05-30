using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Design
{
    public class ComponentRelocatedEventData<TLocation>
        where TLocation : Folders.IFolderHolder
    {
        public ComponentRelocatedEventData(TLocation oldLocation, TLocation newLocation)
        {
            this.OldLocation = oldLocation;
            this.NewLocation = newLocation;
        }

        public TLocation OldLocation { get; private set; }
        public TLocation NewLocation { get; private set; }
    }
}
