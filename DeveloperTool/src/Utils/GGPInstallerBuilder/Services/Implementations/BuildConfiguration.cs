using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;
using Spark.TfsExplorer.Models.Folders;

namespace GGPInstallerBuilder
{
    public class BuildConfiguration : IBuildConfiguration
    {
        public BuildConfiguration(IServerPath ggpApprovalSystemSourceCodeFolder, 
                                  Optional<ILocalPath> installerRootDeliveryFolder,
                                  BuildTaskInfo buildTaskInfo,
                                  ISourceControlAdapter sourceControlAdapter)
        {
            this.GGPApprovalSystemSourceCodeFolder = ggpApprovalSystemSourceCodeFolder;
            _installerRootDeliveryFolder = installerRootDeliveryFolder;
            _buildTaskInfo = buildTaskInfo;
            _sourceControlAdapter = sourceControlAdapter;
        }
        BuildTaskInfo _buildTaskInfo;
        ISourceControlAdapter _sourceControlAdapter;
        public IServerPath GGPApprovalSystemSourceCodeFolder { get; private set; }

    
        public IDictionary<string, string> GlobalProperties
        {
            get
            {
                var buildProperties = new Dictionary<string, string>();
                buildProperties.Add("Configuration", "Debug");
                return buildProperties;
            }
        }

        Optional<ILocalPath> _installerRootDeliveryFolder = Optional<ILocalPath>.None();
        public Optional<ILocalPath> InstallerDeliveryFolder
        {
            get
            {
                if (!_installerRootDeliveryFolder.Any())
                    return Optional<ILocalPath>.None();

                return Optional<ILocalPath>.Some(_installerRootDeliveryFolder.First().Subpath(_buildTaskInfo.Environment.ToString())
                                                                                     .Subpath(_buildTaskInfo.BranchName));
            }
        }

        public ILocalPath InstallerProjectPath
        {
            get
            {
                ILocalPath localGGPApprovalSystemFolder = _sourceControlAdapter.GetLocalPathFromServerPath(GGPApprovalSystemSourceCodeFolder);
                return localGGPApprovalSystemFolder
                       .Subpath("Installer")
                       .Subpath("Projects")
                       .Subpath(_buildTaskInfo.Environment.ToString())
                       .Subpath(_buildTaskInfo.BranchName);
            }
        }
    }
}
