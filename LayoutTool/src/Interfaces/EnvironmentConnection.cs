using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LayoutTool.Interfaces
{
    public sealed class EnvironmentConnection
    {
        public EnvironmentConnection(string name, PathDescriptor configurationFilePath)
        {
            Name = name;
            ConfigurationFilePath = configurationFilePath;
        }

        public string Name { get; private set; }
        public PathDescriptor ConfigurationFilePath { get; private set; }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as EnvironmentConnection;
            if (theOther == null)
                return false;

            return this.Name == theOther.Name;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }
    }
}
