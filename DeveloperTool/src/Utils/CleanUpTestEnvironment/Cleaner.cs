using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.TeamFoundation.Client;
using Microsoft.TeamFoundation.VersionControl.Client;
using Spark.TfsExplorer.Models.TFS;

namespace CleanUpTestEnvironment
{
    class Cleaner
    {
        public Cleaner()
        {
            this.Tfs = TfsCollectionFactory.Create();
        }


        public void Clean()
        {
            RemoveTfsFiles();
            ClearDatabaseRecords();
            RemoveFolders();
        }


        private void RemoveTfsFiles()
        {
            var workspace = GetWorkspace();
            var filesToDelete = new string[]
            {
                @"C:\CasinoTools\GGPDeveloperToolTestData\DEV\Branches\3.x\QA\Main\PublishHistory\LatestPublish.xml",
                @"C:\CasinoTools\GGPDeveloperToolTestData\DEV\Branches\3.x\QA\Main\Trigger\Publish.xml",
                @"C:\CasinoTools\GGPDeveloperToolTestData\DEV\Distribution\Components",
                @"C:\CasinoTools\GGPDeveloperToolTestData\DEV\Distribution\InstallerContent",
                @"C:\CasinoTools\GGPDeveloperToolTestData\DEV\Branches\4.x"
            };

            foreach (var f in filesToDelete)
            {
                if(File.Exists(f) || Directory.Exists(f))
                    workspace.ExecuteVoid(w => w.PendDelete(f, RecursionType.Full));
            }

            var pendingChanges = workspace.GetPendingChanges(filesToDelete, RecursionType.Full);

            if(pendingChanges.Any())
                workspace.CheckIn(pendingChanges, "clean-up publish history");

        }

        TfsTeamProjectCollection Tfs { get; set; }

        VersionControlServer VersionControlServer
        {
            get
            {
                return Tfs.GetService<VersionControlServer>();
            }
        }
        private Workspace GetWorkspace()
        {

            var w = VersionControlServer.QueryWorkspaces("CasinoTools",
                                                        Environment.UserName,
                                                        Environment.MachineName)
                                        .FirstOrDefault();

            if (w == null)
                throw new InvalidOperationException($"There is no TFS workspace named CasinoTools");

            return w;
        }

        private void RemoveFolders()
        {
            var fileSystemManager = new Spark.Infra.Windows.FileSystemManager();
            foreach (var folder in Directory.EnumerateDirectories(@"c:\CasinoTools\GGPDistributionTest"))
            {
                fileSystemManager.DeleteFolder(folder);
            }

            fileSystemManager.DeleteFolder(@"C:\GamingX\Distributions\GGPInstallerFlorin\QA\3.x");
            fileSystemManager.DeleteFolder(@"c:\ProgramData\GGPInstaller");
            fileSystemManager.DeleteFolder(@"C:\CasinoTools\GGPDeveloperToolTestData\DEV\Distribution\InstallerBinaries");
        }

        private void ClearDatabaseRecords()
        {
            using (var sqlConnection = new SqlConnection("Data Source=localhost;Initial Catalog=GGPVersioning;Integrated Security=SSPI"))
            {
                sqlConnection.Open();
                //clear production installers
                var cmd = new SqlCommand("DELETE FROM ProductionInstaller where Branch_ID in (select Branch_ID from Branch where Name in ('3.x', '4.x'))", sqlConnection);
                cmd.ExecuteNonQuery();

                //clear QA installers
                cmd = new SqlCommand("delete from QAInstaller where Branch_ID in (Select Branch_ID From Branch where Name in ('3.x', '4.x'))", sqlConnection);
                cmd.ExecuteNonQuery();

                //clear components
                cmd = new SqlCommand("delete from [dbo].[Component] where Component_ID in (select Component_ID From ComponentToBranchAssignment where Branch_ID in (select Branch_ID from Branch where name in ('3.x', '4.x')))", sqlConnection);
                cmd.ExecuteNonQuery();
                

                sqlConnection.Close();

                
            }
        }
    }
}
