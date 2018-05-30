using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyGamesToOutputDir
{

    public static class WellKnownFolderNames
    {
        public static readonly string GameEngines = "GameEngines";
        public static readonly string Games = "Games";
        public static readonly string Math = "Math";
        public static readonly string Limits = "Limits";
    }

    public class CopyGamesToOutputDir
    {
        public CopyGamesToOutputDir(string componentsFolder, string buildOutputFolder)
        {
            _componentsFolder = componentsFolder;
            _buildOutputFolder = buildOutputFolder;
        }

        string _componentsFolder;
        string _buildOutputFolder;


        public void Run()
        {
            var gameEnginesFolder = Path.Combine(_componentsFolder, WellKnownFolderNames.GameEngines);
            var outputGamesFolder = Path.Combine(_buildOutputFolder, WellKnownFolderNames.Games);

            if (!FolderExists(outputGamesFolder))
                CreateFolder(outputGamesFolder);

            foreach (var gameEngineFolder in Directory.GetDirectories(gameEnginesFolder))
            {
                var gamesFolder = Path.Combine(gameEngineFolder, WellKnownFolderNames.Games);
                if (FolderExists(gamesFolder))
                {
                    foreach (var gameFolder in Directory.GetDirectories(gamesFolder))
                    {
                        CopyGameFiles(gameFolder, outputGamesFolder);
                    }
                }
            }
        }

        private static void CopyGameFiles(string gameFolder, string outputGamesFolder)
        {
            CopyGamePartFiles(gameFolder, WellKnownFolderNames.Math, outputGamesFolder);
            CopyGamePartFiles(gameFolder, WellKnownFolderNames.Limits, outputGamesFolder);
        }

        private static void CopyGamePartFiles(string gameFolder, string categoryName, string outputGamesFolder)
        {
            var categoryFolder = Directory.GetDirectories(gameFolder, categoryName).FirstOrDefault();
            if (!string.IsNullOrEmpty(categoryFolder))
            {
                CopyFolderContent(categoryFolder, 
                                 Path.Combine(outputGamesFolder, new DirectoryInfo(gameFolder).Name),
                                 "ComponentUniqueID.txt");
            }
        }

        private static bool FolderExists(string folder)
        {
            return Directory.Exists(folder);
        }

        private static void CopyFolderContent(string srcFolder, string destFolder, params string[] filesToIgnore )
        {
            foreach (var file in GetFiles(srcFolder))
            {
                string fileName = Path.GetFileName(file);
                if (!filesToIgnore.Contains(fileName, StringComparer.OrdinalIgnoreCase))
                {
                    CopyFile(file, Path.Combine(destFolder, fileName));
                }
            }

            foreach (var srcSubdir in Directory.GetDirectories(srcFolder))
            {
                string destSubdir = Path.Combine(destFolder, new DirectoryInfo(srcSubdir).Name);
                CreateFolder(destSubdir);
                CopyFolderContent(srcSubdir, destSubdir, filesToIgnore);
            }
        }

        private static string[] GetFiles(string folder)
        {
            return Directory.EnumerateFiles(folder).ToArray();
        }

        private static void CreateFolder(string folder)
        {
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);
        }


        private static void CopyFile(string srcFile, string destFile)
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


        private static bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }


        private static void SetAttributesToNormalRecursive(string path)
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
    }
}
