using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spark.Infra.Logging
{
    public interface ILoggerFactory
    {
        ILogger CreateLogger(Type forType);
    }
    
}
