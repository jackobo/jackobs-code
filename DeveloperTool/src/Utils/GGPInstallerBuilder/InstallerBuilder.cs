using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;
using Spark.TfsExplorer.Models.Build;

namespace GGPInstallerBuilder
{
    public class InstallerBuilder
    {
        public InstallerBuilder(IInstallerBuildServices services)
        {
            _services = services;
        }

        ILogger Logger
        {
            get { return _services.LoggerFactory.CreateLogger(this.GetType()); }
        }

        IInstallerBuildServices _services;
        public void Build(BuildTaskInfo taskInfo, string ggpApprovalSystemServerPath, string installerDistributionPath)
        {
            
            var buildConfig = new BuildConfiguration(_services.SourceControlAdapter.CreateServerPath(ggpApprovalSystemServerPath),
                                                     string.IsNullOrEmpty(installerDistributionPath)
                                                        ? Optional<ILocalPath>.None()
                                                        : Optional<ILocalPath>.Some(new LocalPath(installerDistributionPath)),
                                                     taskInfo,
                                                     _services.SourceControlAdapter);

            var buildContext = new InstallerBuildContext(
                                _services.LoggerFactory,
                                _services.FileSystemAdapter,
                                _services.SourceControlAdapter,
                                buildConfig);
            
            
            var actions = GetBuildActionList(taskInfo);

            foreach(var a in actions)
            {
                a.Execute(buildContext);
            }
           
        }

        private ILocalPath GetTempFolder(BuildTaskInfo taskInfo)
        {
            return new LocalPath(Path.GetTempPath())
                                .Subpath(taskInfo.Environment.ToString())
                                .Subpath(taskInfo.BranchName)
                                .Subpath("GGPInstallerTemp");
        }

        private IEnumerable<Actions.IInstallerBuildAction> GetBuildActionList(BuildTaskInfo taskInfo)
        {
            var installerDefinition = _services.InstallerDefinitionReader
                                               .Read(taskInfo);

            Logger.Info($"InstallerVersion = {installerDefinition.Version}; IsCustomizedQAInstaller = {installerDefinition.IsCustomizedInstaller}; Publisher = {installerDefinition.PublisherEmailAddress}; InstallerID = {installerDefinition.InstallerID}");
            
            var tempFolder = GetTempFolder(taskInfo);

            var actions = new List<Actions.IInstallerBuildAction>();

            actions.Add(new Actions.CleanUpTempFolder(tempFolder, true));

            actions.Add(new Actions.GetLatestGGPApprovalSystemSourceCode());
            
            foreach (var component in installerDefinition.Components)
            {
                actions.Add(new Actions.DownloadComponentFilesAction(component, tempFolder));
            }
            
            actions.Add(new Actions.WriteLatestTxtContent(installerDefinition.LatestTxtContent, tempFolder));

            actions.Add(new Actions.ZipComponentsAction(tempFolder));

            actions.Add(new Actions.CompileInstaller());
            
            actions.Add(new Actions.DeliverInstaller(installerDefinition.Version, installerDefinition.IsCustomizedInstaller));

            actions.Add(new Actions.CleanUpTempFolder(tempFolder, false));

            return actions;
        }
    }
}
