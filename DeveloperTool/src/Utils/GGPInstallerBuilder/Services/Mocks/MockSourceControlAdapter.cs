using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;
using Spark.TfsExplorer.Models.Folders;

namespace GGPInstallerBuilder.Mocks
{
    public class MockSourceControlAdapter : ISourceControlAdapter
    {
        public MockSourceControlAdapter(IFileSystemManager fileSystemManager)
        {
            _fileSystemManager = fileSystemManager;
        }
        public IServerPath CreateServerPath(string serverPath)
        {
            return new MockServerPath(serverPath);
        }

        IFileSystemManager _fileSystemManager;
        public void DownloadFolderContent(IServerPath serverPath, ILocalPath localPath, ILogger logger)
        {
            _fileSystemManager.CopyFolderContent(serverPath.AsString(), localPath.AsString());
        }

        public void GetLatest(IServerPath serverPath)
        {
            
        }

        public string ReadTextFile(IServerPath serverPath)
        {
            return _fileSystemManager.ReadAllText(serverPath.AsString());
        }

        public ILocalPath GetLocalPathFromServerPath(IServerPath serverPath)
        {
            return new LocalPath(serverPath.AsString());
        }
    }
}
