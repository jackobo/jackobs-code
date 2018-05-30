using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public class TimeServices : ITimeServices
    {
        public DateTime Now
        {
            get
            {
                return DateTime.Now;
            }
        }
    }
}
