using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public interface IComInteropServices
    {
        void Regsvr32(string comComponentPath);
    }
}
