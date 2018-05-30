using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GamesPortal.Service.Configurations;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.Infra.Configurations;
using Spark.Infra.Logging;
using Spark.Infra.Types;

namespace GamesPortal.Service
{
    public interface ITfsGateway : IDisposable
    {
        ITfsWorkspace GetLayoutToolWorkspace();
        Optional<string> ReadFileContent(string serverFilePath);
        Optional<string> ReadFileContent(string filePath, int value);

        bool FolderExists(string folderPath);
        
    }

    public interface ITfsWorkspace 
    {
        UploadTextFileContentResult UploadTextFileContent(string fileContent, string comments, params string[] fileRelativePathToTheRootWorkspaceFolder);

    }

    public class UploadTextFileContentResult
    {
        public UploadTextFileContentResult(string localFileFullPath, string serverFileFullPath)
        {
            this.LocalFileFullPath = localFileFullPath;
            this.ServerFileFullPath = serverFileFullPath;
        }

        public string LocalFileFullPath { get; private set; }

        public string ServerFileFullPath { get; private set; }
    }


    public class TfsWorkspace : ITfsWorkspace
    {
        public TfsWorkspace(Workspace workspace, string workspaceWorkingFolder)
        {
            this.Workspace = workspace;
            this.WorkspaceWorkingFolder = workspaceWorkingFolder;
        }

        
        Workspace Workspace { get; set; }
        string WorkspaceWorkingFolder { get; set; }

        public UploadTextFileContentResult UploadTextFileContent(string fileContent, string comments, params string[] fileRelativePathToWorkspaceWorkingFolder)
        {
            
            string fileFullName = Path.Combine((new string[] { this.WorkspaceWorkingFolder })
                                                                 .Concat(fileRelativePathToWorkspaceWorkingFolder)
                                                                 .ToArray());
            var parentFolder = Path.GetDirectoryName(fileFullName);

            if (!Directory.Exists(parentFolder))
            {
                Directory.CreateDirectory(parentFolder);
                Workspace.ExecuteAndThrowOnNonFatalError(wks => wks.PendAdd(parentFolder));
            }


            if (File.Exists(fileFullName))
            {
                Workspace.ExecuteAndThrowOnNonFatalError(wks => wks.PendEdit(fileFullName));
                File.WriteAllText(fileFullName, fileContent);

            }
            else
            {
                File.WriteAllText(fileFullName, fileContent);
                Workspace.ExecuteAndThrowOnNonFatalError(wks => wks.PendAdd(fileFullName));
            }
            
            Workspace.CheckIn(Workspace.GetPendingChanges(fileFullName), comments);


            return new UploadTextFileContentResult(fileFullName, Workspace.GetServerItemForLocalItem(fileFullName));


        }
    }
    public class TFSGateway : ITfsGateway
    {
        public TFSGateway(IConfigurationReader configurationReader, ILoggerFactory loggerFactory)
        {
            this.ConfigurationReader = configurationReader;
            this.Logger = loggerFactory.CreateLogger(this.GetType());
            this.Tpc = new TfsTeamProjectCollection(new Uri(configurationReader.ReadSection<Configurations.LayoutToolPublisherSettings>().TfsUrl));
            this.VersionControlServer = Tpc.GetService<VersionControlServer>();
        }

        ILogger Logger { get; set; }

        IConfigurationReader ConfigurationReader { get; set; }

        private TfsTeamProjectCollection Tpc { get; set; }
        private VersionControlServer VersionControlServer
        {
            get; set;
        }
        public void Dispose()
        {
            Tpc.Dispose();
        }

        public ITfsWorkspace GetLayoutToolWorkspace()
        {
            string name = "LayoutTool";
            string workingFolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TFS", "LayoutTool");
            if (!string.IsNullOrEmpty(ConfigurationReader.ReadSection<LayoutToolPublisherSettings>().LocalWorkspaceFolder))
                workingFolder = ConfigurationReader.ReadSection<LayoutToolPublisherSettings>().LocalWorkspaceFolder;
            string serverPath = "$/GamingNDL/Develop/CasinoFlashClient/Configuration/HEAD/LayoutTool";

            var workspace = VersionControlServer.QueryWorkspaces(name, VersionControlServer.AuthorizedUser, Environment.MachineName).FirstOrDefault();

            if (workspace == null)
            {
                workspace = VersionControlServer.CreateWorkspace(name,
                                                            VersionControlServer.AuthorizedUser,
                                                            name,
                                                            new WorkingFolder[]
                                                            {
                                                                new WorkingFolder(serverPath, workingFolder)
                                                            },
                                                            Environment.MachineName);
            }

            return new TfsWorkspace(workspace, workingFolder);
        }

        public Optional<string> ReadFileContent(string serverFilePath)
        {

            var itemSet = VersionControlServer.GetItems(serverFilePath, RecursionType.None);
            if (itemSet.Items.Length == 0)
                return Optional<string>.None();


            using (var stream = itemSet.Items[0].DownloadFile())
            using (var streamReader = new StreamReader(stream))
            {
                return Optional<string>.Some(streamReader.ReadToEnd());
            }

        }

        public Optional<string> ReadFileContent(string serverFilePath, int changeSet)
        {    
            var itemSet = VersionControlServer.GetItems(serverFilePath, new ChangesetVersionSpec(changeSet), RecursionType.None);
            if (itemSet.Items.Length == 0)
                return Optional<string>.None();
            
            using (var stream = itemSet.Items[0].DownloadFile())
            using (var streamReader = new StreamReader(stream))
            {
                return Optional<string>.Some(streamReader.ReadToEnd());
            }
        }

        public bool FolderExists(string folderPath)
        {
            var itemSet = VersionControlServer.GetItems(folderPath, RecursionType.None);
            return itemSet.Items.Length > 0;
        }
    }
}
