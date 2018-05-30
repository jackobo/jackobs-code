using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Exceptions
{
    public class ValidationException : ApplicationException
    {
        public ValidationException(string message)
            : base(message)
        {

        }

        public ValidationException(string message, Exception innerException)
            : base(message, innerException)
        {

        }
    }
}
