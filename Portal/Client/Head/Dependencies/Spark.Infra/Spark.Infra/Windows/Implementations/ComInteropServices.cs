using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Windows
{
    public class ComInteropServices : IComInteropServices
    {
        public void Regsvr32(string comComponentPath)
        {
            ExternalProcessRunner.Run("regsvr32.exe", " /s \"" + comComponentPath + "\"");
        }

    }
}
