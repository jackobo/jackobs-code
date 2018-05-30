using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public interface IFileSystemManager
    {
        void CopyFile(string srcFile, string destFile);

        void DeleteFile(string file);
        bool FilesAreEquals(string file1, string file2);
        string[] GetFiles(string folder);
        void DeleteFolder(string path);
        void CreateFolder(string path);
        void CopyFolderContent(string srcFolder, string destFolder);
        void SetAttributesToNormalRecursive(string path);
        string[] GetDrives();
        bool FileExists(string filePath);
        void DeleteFolderIfHasNoFile(string parentFolder);
        void MoveFile(string srcFile, string destFile);
        string GetTempFileName();
        string GetTempFolder();
        bool FolderExists(string folder);
        System.IO.Stream ReadFileStream(string fullFileName);
        string[] GetSubfolders(string folder);
        string GetMD5HashFromFile(string fileName);

        string ReadAllText(string file);
        void WriteAllText(string file, string content);

        string GetFileExtension(string fileName);

        IEnumerable<string> GetSubFolders(string folder);
        FileStream CreateFileStream(string fileName);

        void ExtractEmbededResourceToFile(string resourceName, System.Reflection.Assembly assembly, string fileName, Action<decimal> progressCallback = null);
        string ExtractEmbededResourceToTempFile(string resourceName, System.Reflection.Assembly assembly, Action<decimal> progressCallback = null);
        void ExtractEmbededZipFileContentToFolder(string resourceName, System.Reflection.Assembly assembly, string destinationFolder);

        void UnzipFile(string zipFile, string toFolder);
    }
}
