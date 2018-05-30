using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class FileSystemAdapter : IFileSystemAdapter
    {
        public FileSystemAdapter(IFileSystemManager fileSystemManager)
        {
            _fileSystemManager = fileSystemManager;
        }

        IFileSystemManager _fileSystemManager;

        public bool FileExists(ILocalPath fileLocalPath)
        {
            return _fileSystemManager.FileExists(fileLocalPath.AsString());
        }

        public void CopyFile(ILocalPath sourceFile, ILocalPath targetFile)
        {
            _fileSystemManager.CopyFile(sourceFile.AsString(), targetFile.AsString());
        }

        public string ReadAllText(ILocalPath filePath)
        {
            return _fileSystemManager.ReadAllText(filePath.AsString());
        }
        
        public void WriteAllText(ILocalPath filePath, string fileContent)
        {
            _fileSystemManager.WriteAllText(filePath.AsString(), fileContent);
        }

        public void WriteFileContent(ILocalPath localPath, byte[] content)
        {
            _fileSystemManager.WriteFileContent(localPath.AsString(), content);
        }

        public void DeleteFile(ILocalPath targetFile)
        {
            _fileSystemManager.DeleteFile(targetFile.AsString());
        }
    }
}
