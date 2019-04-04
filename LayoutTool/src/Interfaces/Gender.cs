using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spark.Infra.Types;


namespace LayoutTool.Interfaces
{
    public class Gender : SmartEnum<string, Gender>
    {
        public Gender(string id, string name)
            : base(id, name)
        {

        }

        public static readonly Gender Male = new Gender("1", "Male");
        public static readonly Gender Fale = new Gender("2", "Female");
    }
}
