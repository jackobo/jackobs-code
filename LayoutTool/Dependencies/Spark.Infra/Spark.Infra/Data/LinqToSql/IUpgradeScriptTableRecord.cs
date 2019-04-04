using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql
{
    public interface IUpgradeScriptTableRecord
    {
        DateTime RunDateTime { get; set; }
        int Script_ID { get; set; }
        string ScriptContent { get; set; }
        string ScriptName { get; set; }

    }
}
