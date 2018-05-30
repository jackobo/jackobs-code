using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models
{
  
    public sealed class LocalPath : ILocalPath
    {
        public LocalPath(string path)
        {
            _path = path;
        }

        string _path;
        public string AsString()
        {
            return _path;
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as LocalPath;
            if (theOther == null)
                return false;

            return 0 == string.Compare(this._path, theOther._path, true);
        }

        public override string ToString()
        {
            return AsString();
        }

        public override int GetHashCode()
        {
            return _path.ToLowerInvariant().GetHashCode();
        }

        public ILocalPath Subpath(string name)
        {
            return new LocalPath(Path.Combine(_path, name));
        }

        public string GetName()
        {
            return _path.Split('\\').Last();
        }
    }
}
