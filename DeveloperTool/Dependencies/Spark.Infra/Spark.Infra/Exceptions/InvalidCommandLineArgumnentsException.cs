using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Exceptions
{
    public class InvalidCommandLineArgumnentsException : ApplicationException
    {
        public InvalidCommandLineArgumnentsException(string usageInfo)
            : this("", usageInfo, null)
        {

        }

        public InvalidCommandLineArgumnentsException(string errorMessage, string usageInfo)
            : this(errorMessage, usageInfo, null)
        {

        }

        public InvalidCommandLineArgumnentsException(string errorMessage, string usageInfo, Exception innerException)
          : base(errorMessage, innerException)
        {
            this.UsageInfo = usageInfo;
        }



        public string UsageInfo { get; private set; }
    }
}
