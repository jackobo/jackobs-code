using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;

namespace Spark.TfsExplorer.Models.Build
{
    public class DeployFileAction : IBuildAction
    {
        public DeployFileAction(IComponentBuilder componentBuilder, IOutputFile outputFile, VersionNumber version)
        {
            _componentBuilder = componentBuilder;
            Version = version;
            OutputFile = outputFile;
        }

        IComponentBuilder _componentBuilder;


        protected VersionNumber Version { get; private set; }
        protected IOutputFile OutputFile { get; private set; }

        public virtual void Execute(IBuildContext buildContext)
        {
            var logger = buildContext.Logger;
            
            var sourceFile = this.OutputFile.ResolveBuildOutputPath(buildContext.BuildConfiguration.OutputFolder);
            var targetFile = this.OutputFile.ResolveDistributionPath(_componentBuilder.ResolveDistributionLocalPath(buildContext.BuildConfiguration.DistributionLocalPath));

            var fileSystemAdapter = buildContext.FileSystemAdapter;
            var sourceControlAdapter = buildContext.SourceControlAdapter;


            if (fileSystemAdapter.FileExists(targetFile)
                && !sourceControlAdapter.HasPendingChanges(targetFile))
            {
                fileSystemAdapter.DeleteFile(targetFile);
            }

            sourceControlAdapter.TryGetLatestVersion(targetFile);
            
            if (fileSystemAdapter.FileExists(targetFile))
            {
                logger.Info($"PendEdit: {targetFile}");
                sourceControlAdapter.CheckoutForEdit(targetFile);
                logger.Info($"Copy {sourceFile} to {targetFile}");
                fileSystemAdapter.CopyFile(sourceFile, targetFile);
            }
            else
            {
                logger.Info($"Copy {sourceFile} to {targetFile}");
                fileSystemAdapter.CopyFile(sourceFile, targetFile);
                logger.Info($"PendAdd: {targetFile}");
                sourceControlAdapter.PendAdd(targetFile);
            }

        }

        
    }
}
