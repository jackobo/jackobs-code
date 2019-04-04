using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace GGPMockBootstrapper.Models
{
    public interface ISwfFilesProvider
    {
        SwfFile[] GetSwfFiles(int forGameType);

        void UpdateSelectedSwf(int p, SwfFile swf);
    }

    public class SwfFile
    {
        public SwfFile(string fullName)
        {
            this.FullName = fullName;
        }
        public string FullName { get; private set; }
        public string Name
        {
            get { return Path.GetFileName(FullName); }
        }

        public override bool Equals(object obj)
        {
            var theOther = obj as SwfFile;

            if (theOther == null)
                return false;

            return 0 == string.Compare(this.Name, theOther.Name, true);
        }

        public override string ToString()
        {
            return this.Name;
        }

        public override int GetHashCode()
        {
            return this.Name.ToLowerInvariant().GetHashCode();
        }

        public bool IsSelected { get; set; }
    }
}
