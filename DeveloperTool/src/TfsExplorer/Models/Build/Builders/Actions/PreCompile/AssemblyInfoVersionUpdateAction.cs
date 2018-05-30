using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Spark.Infra.Types;
using Spark.Infra.Windows;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.TFS;

namespace Spark.TfsExplorer.Models.Build
{
    public class AssemblyInfoVersionUpdateAction : IBuildAction
    {
        private static Regex _versionRecognizer = new Regex("\"\\d+.\\d+.\\d+.\\d+\"");
        public AssemblyInfoVersionUpdateAction(ILocalPath assemblyInfoFile, VersionNumber version)
        {
            _assemblyInfoFile = assemblyInfoFile;
            _version = version;
        }

        public ILocalPath _assemblyInfoFile;
        VersionNumber _version;

        public void Execute(IBuildContext buildContext)
        {
            var logger = buildContext.Logger;
            logger.Info($"Checkout for edit file: {_assemblyInfoFile}");
            buildContext.SourceControlAdapter.CheckoutForEdit(_assemblyInfoFile);
            logger.Info($"Set version {_version} into file {_assemblyInfoFile}");
            ChangeAssemblyInfoVersion(buildContext.FileSystemAdapter);
        }

        private void ChangeAssemblyInfoVersion(IFileSystemAdapter fileSystemAdapter)
        {
            var originalfileContent = fileSystemAdapter.ReadAllText(_assemblyInfoFile);
            var newFileContent = _versionRecognizer.Replace(originalfileContent, "\"" + _version.ToString() + "\"");
            fileSystemAdapter.WriteAllText(_assemblyInfoFile, newFileContent);
        }
    }
}
