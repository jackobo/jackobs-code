using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public interface IFileSystemAdapter
    {
        bool FileExists(ILocalPath fileLocalPath);
        void CopyFile(ILocalPath sourceFile, ILocalPath targetFile);
        string ReadAllText(ILocalPath filePath);
        void WriteAllText(ILocalPath filePath, string fileContent);
        void WriteFileContent(ILocalPath localPath, byte[] content);
        void DeleteFile(ILocalPath targetFile);
    }
}
