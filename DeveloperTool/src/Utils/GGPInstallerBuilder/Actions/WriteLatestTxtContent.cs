using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Logging;
using Spark.TfsExplorer.Interfaces;

namespace GGPInstallerBuilder.Actions
{
    public class WriteLatestTxtContent : IInstallerBuildAction
    {
        public WriteLatestTxtContent(string content, ILocalPath toFolder)
        {
            _content = content;
            _toFolder = toFolder;
        }

        string _content;
        ILocalPath _toFolder;

        public void Execute(IInstallerBuildContext context)
        {
            var path = _toFolder.Subpath(Constants.LatestTxt);
            context.Logger.Info($"Writting content for {path.AsString()}");
            context.FileSystemAdapter.WriteTextFileContent(path, _content);
        }
    }
}
