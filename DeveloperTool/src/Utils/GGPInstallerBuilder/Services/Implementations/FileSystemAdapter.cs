using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;

namespace GGPInstallerBuilder
{
    public class FileSystemAdapter : IFileSystemAdapter
    {
        public FileSystemAdapter(IFileSystemManager fileSystemManager)
        {
            _fileSystemManager = fileSystemManager;
        }
        IFileSystemManager _fileSystemManager;

        public void CopyFolder(ILocalPath sourceFolder, ILocalPath targetFolder)
        {
            _fileSystemManager.CopyFolderContent(sourceFolder.AsString(), targetFolder.AsString());
        }

        public void DeleteFolder(ILocalPath folder)
        {
            _fileSystemManager.DeleteFolder(folder.AsString());
        }

        public ILocalPath[] FindFiles(ILocalPath folder, string filter)
        {
            return Directory.EnumerateFiles(folder.AsString(), filter)
                     .Select(f => new LocalPath(f))
                     .ToArray();
        }

        public bool FolderExists(ILocalPath folder)
        {
            return _fileSystemManager.FolderExists(folder.AsString());
        }

       

        public void WriteTextFileContent(ILocalPath localPath, string content)
        {
            _fileSystemManager.WriteAllText(localPath.AsString(), content);
        }

        public void ZipFolderContent(ILocalPath sourceFolder, ILocalPath resultingZipFile)
        {
            _fileSystemManager.ZipFolderContent(sourceFolder.AsString(), resultingZipFile.AsString());
        }
    }
}
