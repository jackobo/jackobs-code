using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder
{
    public interface IFileSystemAdapter
    {
       
        void WriteTextFileContent(ILocalPath localPath, string content);
        void ZipFolderContent(ILocalPath sourceFolder, ILocalPath resultingZipFile);
        ILocalPath[] FindFiles(ILocalPath installerProjectPath, string v);
        void DeleteFolder(ILocalPath folder);
        bool FolderExists(ILocalPath folder);
        void CopyFolder(ILocalPath sourceFolder, ILocalPath targetFolder);
    }
}
