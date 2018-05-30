using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder.Actions
{
    public class DownloadComponentFilesAction : IInstallerBuildAction
    {
        public DownloadComponentFilesAction(InstallerDefinition.ComponentDefinition component, ILocalPath toFolder)
        {
            _component = component;
            _toFolder = toFolder;
        }

        InstallerDefinition.ComponentDefinition _component;
        ILocalPath _toFolder;
        public void Execute(IInstallerBuildContext context)
        {
            context.Logger.Info($"Start download component from {_component.DistributionFolder.AsString()}");
            context.SourceControlAdapter.DownloadFolderContent(_component.DistributionFolder, 
                                                               _toFolder.Subpath(_component.ComponentID),
                                                               context.Logger);
        }
    }
}
