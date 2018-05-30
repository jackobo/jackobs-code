using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Interfaces.Events
{
   
    public class ComponentDeletedEventData
    {
        public ComponentDeletedEventData(ILogicalComponent component)
        {
            this.Component = component;
        }

        public ILogicalComponent Component { get; private set; }
    }
}
