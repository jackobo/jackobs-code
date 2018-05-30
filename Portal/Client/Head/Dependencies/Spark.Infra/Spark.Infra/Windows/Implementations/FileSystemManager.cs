using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;


namespace Spark.Infra.Windows
{
    public class FileSystemManager : IFileSystemManager
    {
        
        public FileSystemManager()
        {
        }

        

        public void CopyFile(string srcFile, string destFile)
        {

            if (FileExists(destFile))
            {
                SetAttributesToNormalRecursive(destFile);
            }
            else
            {
                CreateFolder(Path.GetDirectoryName(destFile));
            }


            File.Copy(srcFile, destFile, true);
        }

        

        public bool FolderExists(string folder)
        {
            return Directory.Exists(folder);
        }



        public void MoveFile(string srcFile, string destFile)
        {
            File.Move(srcFile, destFile);
        }


        public string GetTempFileName()
        {
            var tempFile = Path.GetTempFileName();

            if (this.FileExists(tempFile))
                this.DeleteFile(tempFile);

            return tempFile;
        }

        public string GetTempFolder()
        {
            return Path.GetTempPath();
        }

        public void DeleteFolderIfHasNoFile(string folder)
        {
            if (CountFiles(folder) == 0)
            {
                DeleteFolder(folder);
            }

        }

        private int CountFiles(string folder)
        {
            int filesCount = 0;

            foreach (var subdir in Directory.EnumerateDirectories(folder))
                filesCount += CountFiles(subdir);

            return filesCount + Directory.EnumerateFiles(folder).Count();
        }


        public string[] GetDrives()
        {
            return Directory.GetLogicalDrives();
        }


        public bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }


        public string[] GetFiles(string folder)
        {
            return Directory.EnumerateFiles(folder).ToArray();
        }


        public void DeleteFolder(string folder)
        {
            if (Directory.Exists(folder))
            {
                SetAttributesToNormalRecursive(folder);
                Directory.Delete(folder, true);
            }
        }

        public void SetAttributesToNormalRecursive(string path)
        {
            if (Directory.Exists(path))
            {
                var dirInfo = new DirectoryInfo(path);
                dirInfo.Attributes = FileAttributes.Normal;
                foreach (var fileInfo in dirInfo.EnumerateFiles())
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                }

                foreach (var subfolder in dirInfo.EnumerateDirectories())
                {
                    SetAttributesToNormalRecursive(subfolder.FullName);
                }
            }
            else if (File.Exists(path))
            {
                File.SetAttributes(path, FileAttributes.Normal);
            }
        }
        
        
        public void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }

        public void CopyFolderContent(string srcFolder, string destFolder)
        {
            foreach (var file in GetFiles(srcFolder))
            {
                CopyFile(file, Path.Combine(destFolder, Path.GetFileName(file)));
            }

            foreach (var srcSubdir in Directory.GetDirectories(srcFolder))
            {
                string destSubdir = Path.Combine(destFolder, new DirectoryInfo(srcSubdir).Name);
                CreateFolder(destSubdir);
                CopyFolderContent(srcSubdir, destSubdir);
            }
        }

        public void DeleteFile(string file)
        {
            SetAttributesToNormalRecursive(file);
            File.Delete(file);
        }

        public bool FilesAreEquals(string file1, string file2)
        {

            FileInfo f1Info = new FileInfo(file1);
            FileInfo f2Info = new FileInfo(file2);


            if (!f1Info.Exists || !f2Info.Exists)
                return false;

            if (f1Info.Length != f2Info.Length)
                return false;



            using (var f1Stream = f1Info.OpenRead())
            using (var f2Stream = f2Info.OpenRead())
            {
                f1Stream.Position = 0;
                f2Stream.Position = 0;

                int b1 = f1Stream.ReadByte();
                while (b1 != -1)
                {
                    var b2 = f2Stream.ReadByte();

                    if (b1 != b2)
                        return false;

                    b1 = f1Stream.ReadByte();
                }

            }


            return true;
        }


        public System.IO.Stream ReadFileStream(string fullFileName)
        {
            return File.OpenRead(fullFileName);
        }

        public string GetMD5HashFromFile(string fileName)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(fileName))
                {
                    return BitConverter.ToString(md5.ComputeHash(stream)).Replace("-", string.Empty);
                }
            }
        }

        public string[] GetSubfolders(string folder)
        {
            return Directory.GetDirectories(folder);
        }

        
        public string ReadAllText(string file)
        {
            return File.ReadAllText(file);
        }


        public void WriteAllText(string file, string content)
        {
            File.WriteAllText(file, content);
        }


        public string GetFileExtension(string fileName)
        {
            return Path.GetExtension(fileName);
        }




        public IEnumerable<string> GetSubFolders(string folder)
        {
            return Directory.EnumerateDirectories(folder);
        }
        
        public FileStream CreateFileStream(string fileName)
        {
            if (!Directory.Exists(Path.GetDirectoryName(fileName)))
                Directory.CreateDirectory(Path.GetDirectoryName(fileName));

            if (File.Exists(fileName))
                File.Delete(fileName);

            return File.Create(fileName);
        }


        public void ExtractEmbededZipFileContentToFolder(string resourceName, System.Reflection.Assembly assembly, string destinationFolder)
        {
            var zipFileName = this.GetTempFileName();

            this.ExtractEmbededResourceToFile(resourceName, assembly, zipFileName, null);

            using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(zipFileName))
            {
                zipFile.ExtractAll(destinationFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
            }

            this.DeleteFile(zipFileName);
        }

        public string ExtractEmbededResourceToTempFile(string resourceName, System.Reflection.Assembly assembly, Action<decimal> progressCallback = null)
        {
            string tempFileName = this.GetTempFileName();
            ExtractEmbededResourceToFile(resourceName, assembly, tempFileName, progressCallback);
            return tempFileName;
        }

        public void ExtractEmbededResourceToFile(string resourceName, System.Reflection.Assembly assembly, string fileName, Action<decimal> progressCallback = null)
        {

            using (var fileStream = this.CreateFileStream(fileName))
            {
                int numberOfBytesToRead = 1024;
                byte[] buffer = new byte[numberOfBytesToRead];

                long totalBytesRead = 0;
                using (var stream = assembly.GetManifestResourceStream(resourceName))
                {

                    int readBytes = 0;
                    do
                    {
                        readBytes = stream.Read(buffer, 0, numberOfBytesToRead);
                        fileStream.Write(buffer, 0, readBytes);
                        totalBytesRead += readBytes;

                        if (progressCallback != null)
                            progressCallback((decimal)totalBytesRead / (decimal)stream.Length);


                    } while (readBytes > 0);
                }
            }
        }


        public void UnzipFile(string zipFileName, string toFolder)
        {
            if (!FolderExists(toFolder))
                CreateFolder(toFolder);

            using (Ionic.Zip.ZipFile zipFile = Ionic.Zip.ZipFile.Read(zipFileName))
            {
                zipFile.ExtractAll(toFolder, Ionic.Zip.ExtractExistingFileAction.OverwriteSilently);
            }

        }
    }
}
