using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models
{
    public class FileBasedComponentUniqueID : IComponentUniqueId
    {
        /*public FileBasedComponentUniqueID(Folders.IFileHolder componentUniqueIdTxtFile)
            : this(() => componentUniqueIdTxtFile)
        {
        }
        */

        public FileBasedComponentUniqueID(Func<Folders.IFileHolder> componentUniqueIdTxtFile)
        {
            _componentUniqueIdTxtFile = componentUniqueIdTxtFile;
        }


        Func<Folders.IFileHolder> _componentUniqueIdTxtFile;


        string _value = null;
        public string Value
        {
            get
            {
                if (string.IsNullOrEmpty(_value))
                {
                    LazyLoadUniqueId();
                }

                return _value;
            }
        }

        private void LazyLoadUniqueId()
        {
            var uniqueIdFile = _componentUniqueIdTxtFile();
            if (uniqueIdFile.Exists())
            {
                var localFile = uniqueIdFile.ToSourceControlFile().GetLocalPath().AsString();
                if (File.Exists(localFile))
                    _value = File.ReadAllText(localFile)?.Trim();
                else
                    _value = uniqueIdFile.GetTextContent()?.Trim();
            }
            else
                _value = Guid.NewGuid().ToString();
        }


        public override string ToString()
        {
            return $"{Value} : {_componentUniqueIdTxtFile().GetServerPath().AsString()}";
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as FileBasedComponentUniqueID;
            if (theOther == null)
                return false;

            return this.Value == theOther.Value;
        }

        public override int GetHashCode()
        {
            return this.Value.GetHashCode();
        }
    }
}
