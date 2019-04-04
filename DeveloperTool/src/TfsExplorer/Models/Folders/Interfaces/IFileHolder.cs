using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Folders
{
    public interface IFileHolder
    {
        string Name { get; }
        bool Exists();
        IServerPath GetServerPath();
        ISourceControlFile ToSourceControlFile();


        void SetBinaryContent(byte[] content);
        byte[] GetBinaryContent();

        void SetTextContent(string content);
        string GetTextContent();
        
    }
}
