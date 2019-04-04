using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.TfsExplorer.Interfaces;
using Spark.TfsExplorer.Models;

namespace GGPInstallerBuilder.Mocks
{
    public class MockServerPath : IServerPath
    {
        public MockServerPath(string serverPath)
        {
            _localPath = new LocalPath(serverPath.Replace("$", "C:").Replace("/", "\\"));
        }

        ILocalPath _localPath;

        public string AsString()
        {
            return _localPath.AsString();
        }

        public string GetName()
        {
            return _localPath.GetName();
        }

        public IServerPath Subpath(string name)
        {
            return new MockServerPath(_localPath.Subpath(name).AsString());
        }
    }
}
