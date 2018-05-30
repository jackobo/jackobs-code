using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.TfsExplorer.Models.Build
{
    public class CheckOutputFileExistAction : IBuildAction
    {
        public CheckOutputFileExistAction(IOutputFile outputFile)
        {
            this.OutputFile = outputFile;
        }
        
        protected IOutputFile OutputFile { get; private set; }

        public void Execute(IBuildContext buildContext)
        {
            var fileFullPath = this.OutputFile.ResolveBuildOutputPath(buildContext.BuildConfiguration.OutputFolder);

            if (!buildContext.FileSystemAdapter.FileExists(fileFullPath))
                throw new ApplicationException($"Missing file {fileFullPath}");

        }
    }
}
