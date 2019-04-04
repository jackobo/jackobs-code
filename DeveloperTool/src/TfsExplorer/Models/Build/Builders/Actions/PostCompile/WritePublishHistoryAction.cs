using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models.Folders;

namespace Spark.TfsExplorer.Models.Build
{
    public class WritePublishHistoryAction : IBuildAction
    {
        private IFileHolder _latestPublishXml;
        private byte[] _fileContent;

        public WritePublishHistoryAction(IFileHolder latestPublishXml, byte[] fileContent)
        {
            _fileContent = fileContent;
            _latestPublishXml = latestPublishXml;
        }

        public void Execute(IBuildContext buildContext)
        {
            buildContext.Logger.Info($"Write {_latestPublishXml.GetServerPath().AsString()}");
            _latestPublishXml.SetBinaryContent(_fileContent);
        }
    }
}
