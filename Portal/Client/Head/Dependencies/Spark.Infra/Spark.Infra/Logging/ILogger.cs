using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Spark.Infra.Logging
{
    public interface ILogger
    {   
        void Exception(string message, Exception ex);
        void Exception(Exception ex);
        void Error(string message);
        void Warning(string message);
        void Info(string message);
    }

}
