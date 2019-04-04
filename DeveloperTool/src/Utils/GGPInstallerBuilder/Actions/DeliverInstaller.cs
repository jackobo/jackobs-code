using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder.Actions
{
    public class DeliverInstaller : IInstallerBuildAction
    {
        public DeliverInstaller(VersionNumber installerVersion, bool isCustomizedInstaller)
        {
            _installerVersion = installerVersion;
            _isCustomizedInstaller = isCustomizedInstaller;
        }

        VersionNumber _installerVersion;
        bool _isCustomizedInstaller;

        public void Execute(IInstallerBuildContext buildContext)
        {
            var sourceFolder = buildContext.BuildConfiguration.InstallerProjectPath
                                                                     .Subpath("bin")
                                                                     .Subpath("Debug");

            buildContext.BuildConfiguration.InstallerDeliveryFolder.Do(
                targetFolder =>
                {
                    buildContext.Logger.Info($"Deliver installer to {targetFolder.AsString()}");

                    if (_isCustomizedInstaller)
                        targetFolder = targetFolder.Subpath("Customized");
                    else
                        targetFolder = targetFolder.Subpath("Regular");

                    targetFolder = targetFolder.Subpath(_installerVersion.ToString());

                    buildContext.FileSystemAdapter.CopyFolder(sourceFolder, targetFolder);
                });
        }
    }
}
