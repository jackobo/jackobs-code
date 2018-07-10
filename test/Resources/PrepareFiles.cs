using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace Spark.TfsExplorer.Models.Build
{
    public class PrepareFiles : IBuildAction
    {
        static FileHandler[] _fileHandlers = new FileHandler[0];
        public void Execute(IBuildContext buildContext)
        {
            try
            {
                var files = new Files(buildContext.BuildConfiguration.ComponentsFolder);
                _fileHandlers = files.GetFiles();

                if(!CanHandle())
                {
                    return;
                }

                BackupFiles();
                ClearReadonly();
                DoReplacements();
            }
            catch
            {
                buildContext.Logger.Info("Files prepared!");
            }
        }

        public static void RollbackAllFiles()
        {
            try
            {
                if(_fileHandlers == null)
                {
                    return;
                }

                foreach (var f in _fileHandlers)
                {
                    f.Rollback();
                }
            }
            catch
            {

            }
        }

        private void DoReplacements()
        {
            foreach (var f in _fileHandlers)
            {
                f.DoReplacements();
            }
        }

        private void ClearReadonly()
        {
            foreach(var f in _fileHandlers)
            {
                f.ClearReadonly();
            }
        }

        private void BackupFiles()
        {
            var tempFolder = Path.GetTempPath();
            foreach(var f in _fileHandlers)
            {
                f.Backup(tempFolder);
            }
        }

        bool CanHandle()
        {
            foreach(var f in _fileHandlers)
            {
                if (!f.CanHandle())
                    return false;
            }

            return true;
        }

        private class Files
        {
            public Files(ILocalPath componentsFolder)
            {
                _componentsFolder = componentsFolder;
            }

            ILocalPath _componentsFolder;

            public FileHandler[] GetFiles()
            {
                return new FileHandler[]
                {
                    IApprovalSystemSupportService,
                    ApprovalSystemSupportService,
                    PlayerSessionManager,
                    ICoreOperationService,
                    CoreOperationService,
                    TableHandler
                };
            }

            private string ApprovalSystemSupportServiceFolder
            {
                get
                {
                    return Path.Combine(_componentsFolder.AsString(),
                                        "Core", "GGPHost", "Host", "Services", "ApprovalSystemSupportService");
                }
            }

            public FileHandler IApprovalSystemSupportService
            {
                get
                {
                    return new FileHandler(Path.Combine(ApprovalSystemSupportServiceFolder, "IApprovalSystemSupportService.cs"),
                                           "interface IApprovalSystemSupportService",
                                           " partial interface IApprovalSystemSupportService");
                }
            }

            public FileHandler ApprovalSystemSupportService
            {
                get
                {
                    
                    return new FileHandler(Path.Combine(ApprovalSystemSupportServiceFolder, "ApprovalSystemSupportService.cs"),
                                           "class ApprovalSystemSupportService",
                                           " partial class ApprovalSystemSupportService",
                                           "ApprovalSystemSupportService.txt");
                }
            }
            private string GGPGameServerCoreFolder
            {
                get
                {
                    return Path.Combine(_componentsFolder.AsString(), "Core", "GGPCore", "GGPGameServer.Core");
                }
            }

            public FileHandler PlayerSessionManager
            {
                get
                {

                    return new FileHandler(Path.Combine(GGPGameServerCoreFolder, "PlayerSessionManager", "PlayerSessionManager.cs"),
                                           "if (DIFactory.PlayerSessionDAL.Put(UniquePlayerID.ToString(), pS) == false)",
                                           "PIayerSessionManager.MapCidToSessionId(pS.ClientID, UniquePlayerID);" 
                                           + Environment.NewLine 
                                           + "if (DIFactory.PlayerSessionDAL.Put(UniquePlayerID.ToString(), pS) == false)",
                                           "PlayerSessionManager.txt");
                }
            }

            public FileHandler CoreOperationService
            {
                get
                {
                    return new FileHandler(Path.Combine(GGPGameServerCoreFolder, "WCFServices", "CoreOperationsService", "CoreOperationsService.cs"),
                                           "class CoreOperationsService",
                                           " partial class CoreOperationsService",
                                           "CoreOperationsService.txt");
                }
            }

            public FileHandler ICoreOperationService
            {
                get
                {
                    return new FileHandler(Path.Combine(GGPGameServerCoreFolder, "WCFServices", "CoreOperationsService", "ICoreOperationsService.cs"),
                                            "interface ICoreOperationsService",
                                            "partial interface ICoreOperationsService");
                }
            }

            public FileHandler TableHandler
            {
                get
                {
                    return new FileHandler(Path.Combine(_componentsFolder.AsString(), "GameEngines", "Roulette", "Engine", "GameEngine", "TableHandler.cs"),
                                           "long[] result = GameEngine.ServiceBridge.GetRandomNumbers(GameId, RoulettePlayer.PlayerID, 1, GameEngine.SpinMinimum, GameEngine.SpinMaximum, true);",
                                           "long[] result = RouletteRng.Generate(new RouletteRng.GenerateNumberRequest(GameEngine, RoulettePlayer, GameId, TableStateSnapshot));");
                }
            }
        }
        

        private class FileHandler
        {
            public FileHandler(string path, string find, string replace, string append = null)
            {
                _path = path;
                _find = find;
                _replace = replace;
                _append = append;
            }

            string _path;
            string _find;
            string _replace;
            string _append;

            public bool CanHandle()
            {
                if(!File.Exists(_path))
                {
                    return false;
                }

                if(!File.ReadAllText(_path).Contains(_find))
                {
                    return false;
                }

               
                return true;
            }

            string _backupFilePath;
            public void Backup(string tempFolder)
            {
                _backupFilePath = Path.Combine(tempFolder, Path.GetFileName(_path));
                if(File.Exists(_backupFilePath))
                {
                    File.Delete(_backupFilePath);
                }

                File.Copy(_path, _backupFilePath, true);
            }

            public void ClearReadonly()
            {
                new FileInfo(_path).Attributes = FileAttributes.Normal;
            }

            public void DoReplacements()
            {
                var fileContent = File.ReadAllText(_path);
                fileContent = fileContent.Replace(_find, _replace);

                if(!string.IsNullOrEmpty(_append))
                {
                    var sb = new StringBuilder(fileContent);
                    sb.AppendLine(GetTextToAppend());
                    fileContent = sb.ToString();
                }
                
                File.WriteAllText(_path, fileContent);

            }

            private string GetTextToAppend()
            {
                var assembly = typeof(PrepareFiles).Assembly;
                
                using (var stream = assembly.GetManifestResourceStream("Spark.TfsExplorer.Models.Build.Builders.Actions.PreCompile.Resources." + _append))
                using (var streamReader = new StreamReader(stream, Encoding.Unicode))
                {

                    return streamReader.ReadToEnd();
                }
            }

            string GetResourceName()
            {
                return "Spark.TfsExplorer.Models.Build.Builders.Actions.PreCompile.Resources." + _append;
            }

            public void Rollback()
            {
                if(string.IsNullOrEmpty(_backupFilePath))
                {
                    return;
                }

                File.Copy(_backupFilePath, _path, true);
            }
        }
    }


}
