using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    
    public class RootBranchFolder : IRootFolder
    {
        public RootBranchFolder(IServerPath parentPath, ISourceControlFolder folder)
        {
            _parentPath = parentPath;
            _folder = folder;
        }


        IServerPath _parentPath;

        ISourceControlFolder IFolderHolder.Create()
        {
            return _folder;
        }

        ISourceControlFolder _folder;

        public ISourceControlFolder ToSourceControlFolder()
        {
            return _folder;
        }

        public Optional<ISourceControlFolder> TryGetSubfolder(string subfolderName)
        {
            return ToSourceControlFolder().TryGetSubfolder(subfolderName);
        }

        public string Name
        {
            get
            {
                return _folder.Name;
            }
        }


        public bool Exists()
        {
            return true;
        }

        public IServerPath GetServerPath()
        {
            return _parentPath.Subpath(this.Name);
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            var theOther = obj as RootBranchFolder;

            return this.GetServerPath().Equals(theOther.GetServerPath());
        }

        public override string ToString()
        {
            return this.GetServerPath().ToString();
        }

        public override int GetHashCode()
        {
            return this.GetServerPath().GetHashCode();
        }

        public QAFolder QA
        {
            get { return new QAFolder(this); }
        }

        public DevFolder DEV
        {
            get { return new DevFolder(this); }
        }

        public ProdFolder PROD
        {
            get { return new ProdFolder(this); }
        }

        IRootFolder IFolderHolder.Root
        {
            get { return this; }
        }
    }
    
}
