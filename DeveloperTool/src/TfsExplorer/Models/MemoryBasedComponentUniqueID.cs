using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models
{
    public class MemoryBasedComponentUniqueID : IComponentUniqueId
    {
        public MemoryBasedComponentUniqueID(string value)
        {
            this.Value = value;
        }

        public MemoryBasedComponentUniqueID()
            : this(Guid.NewGuid().ToString())
        {

        }
        public string Value { get; private set; }

        public override bool Equals(object obj)
        {
            var theOther = obj as IComponentUniqueId;
            if (theOther == null)
                return false;

            return this.Value == theOther.Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }

        public override string ToString()
        {
            return this.Value.ToString();
        }
    }
}
