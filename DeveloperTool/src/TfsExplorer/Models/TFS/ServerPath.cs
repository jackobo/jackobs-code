using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.TFS
{
#warning add some unit tests 
    public class ServerPath : IServerPath
    {
        public ServerPath(string path)
        {
            if (path.EndsWith("/"))
                _path = path.Substring(0, path.Length - 1);
            else
                _path = path;
            
            _name = _path.Split('/').Last();
        }

        string _path;

        public string AsString()
        {
            return _path;
        }

        public override string ToString()
        {
            return AsString();
        }


        public override bool Equals(object obj)
        {
            var theOther = obj as ServerPath;

            if (theOther == null)
                return false;

            return _path.Equals(theOther._path, StringComparison.OrdinalIgnoreCase);
        }

        public override int GetHashCode()
        {
            return _path.ToLowerInvariant().GetHashCode();
        }

        public IServerPath Subpath(string name)
        {
            if(name.StartsWith("/"))
                return new ServerPath(_path + name);
            else
                return new ServerPath(_path + "/" + name);
        }

        private string _name;
        public string GetName()
        {
            return _name;
        }
    }
}
