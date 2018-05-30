using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra.Logging;

namespace GamesPortal.Service.Artifactory
{
    public interface IArtifactorySyncronizationLogger
    {
        void Info(string message);
        void Warn(string message);
        void Error(string message);
        void Flush(ILogger logger);
    }
}
