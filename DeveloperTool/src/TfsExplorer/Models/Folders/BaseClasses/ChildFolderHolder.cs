using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public abstract class ChildFolderHolder<TConcreteFolder, TParent> : IFolderHolder
        where TParent : IFolderHolder
        where TConcreteFolder : IFolderHolder
    {
        public ChildFolderHolder(string name, TParent parent)
        {
            _name = name;
            this.Parent = parent;
        }

        string _name;
        public virtual string Name
        {
            get { return _name; }
        }


        public IRootFolder Root
        {
            get { return this.Parent.Root; }
        }

        ISourceControlFolder IFolderHolder.Create()
        {
            var folder = TryConvertToSourceControlFolder();
            if (folder.Any())
                return folder.First();

            return Parent.Create().CreateSubfolder(Name);
        }

        public virtual TParent Parent { get; private set; }
        

        public virtual bool Exists()
        {
            return TryConvertToSourceControlFolder().Any();
        }

        public Optional<ISourceControlFolder> TryGetSubfolder(string subfolderName)
        {

            var result = Optional<ISourceControlFolder>.None();

            TryConvertToSourceControlFolder().Do(scf =>
            {
                result = scf.TryGetSubfolder(subfolderName);
            });

            return result;
        }

        public ISourceControlFolder ToSourceControlFolder()
        {
            var folder = TryConvertToSourceControlFolder();
            if (!folder.Any())
                throw new InvalidOperationException($"Folder {GetServerPath().AsString()} doesn't exists!");

            return folder.First();
        }

        protected Optional<ISourceControlFolder> TryConvertToSourceControlFolder()
        {
            return Parent.TryGetSubfolder(this.Name);
        }

        public virtual TConcreteFolder Create()
        {
            ((IFolderHolder)this).Create();
            return (TConcreteFolder)(object)this;
        }

        public IServerPath GetServerPath()
        {
            return Parent.GetServerPath().Subpath(this.Name);
        }
        
    

       

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (this.GetType() != obj.GetType())
                return false;

            var theOther = obj as ChildFolderHolder<TConcreteFolder, TParent>;

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
    }
}
