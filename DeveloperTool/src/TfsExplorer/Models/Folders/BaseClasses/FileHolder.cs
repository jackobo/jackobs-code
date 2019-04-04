using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public class FileHolder<TParent> : IFileHolder
        where TParent : IFolderHolder
    {
        public FileHolder(string name, TParent parent)
        {
            this.Name = name;
            this.Parent = parent;
        }

        public TParent Parent { get; private set; }

        public string Name { get; private set; }

        public bool Exists()
        {
            return TryGetSourceControlFile().Any();
        }

        private Optional<ISourceControlFile> TryGetSourceControlFile()
        {
            if (!this.Parent.Exists())
                return Optional<ISourceControlFile>.None();

            return this.Parent.ToSourceControlFolder().TryGetFile(this.Name);
        }

        public ISourceControlFile ToSourceControlFile()
        {
            var file = TryGetSourceControlFile();
            if (!file.Any())
                throw new InvalidOperationException($"The file {this.GetServerPath().AsString()} doesn't exists!");

            return file.First();
        }

        public IServerPath GetServerPath()
        {
            return this.Parent.GetServerPath().Subpath(this.Name);
        }


        public Optional<ILocalPath> GetLocalPath()
        {
            if (this.Exists())
                return Optional<ILocalPath>.Some(this.ToSourceControlFile().GetLocalPath());

            return Optional<ILocalPath>.None();
        }

        public void SetBinaryContent(byte[] content)
        {
            if(Exists())
            {
                this.ToSourceControlFile().UpdateContent(content);
            }
            else
            {
                this.Parent.Create().CreateFile(this.Name, content);
            }
        }

        public byte[] GetBinaryContent()
        {
            if (!this.Exists())
                throw new InvalidOperationException($"Can't get the content for the file {this.GetServerPath().AsString()} because this file doesn't exists!");


            return this.ToSourceControlFile().GetContent();
        }

        public void SetTextContent(string content)
        {
            SetBinaryContent(Encoding.UTF8.GetBytes(content));
        }

        public string GetTextContent()
        {
            return Encoding.UTF8.GetString(GetBinaryContent());
        }
    }
}
