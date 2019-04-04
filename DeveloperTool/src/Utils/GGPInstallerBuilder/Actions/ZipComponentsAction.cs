using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder.Actions
{
    public class ZipComponentsAction : IInstallerBuildAction
    {
        private ILocalPath _sourceFolder;
        
        public ZipComponentsAction(ILocalPath sourceFolder)
        {
            _sourceFolder = sourceFolder;
        }

        public void Execute(IInstallerBuildContext context)
        {            
            var zipFileFullPath = context.BuildConfiguration.InstallerProjectPath.Subpath(Constants.GGPFullBuildZip);
            context.Logger.Info($"Zipping folder {_sourceFolder.AsString()} to {zipFileFullPath.AsString()}");
            context.FileSystemAdapter.ZipFolderContent(_sourceFolder, zipFileFullPath);
        }
    }
}
