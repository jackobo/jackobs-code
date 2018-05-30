using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Spark.Infra.Data.LinqToSql;

namespace GamesPortal.Service.DataAccessLayer
{
    public partial class UpgradeScript : IUpgradeScriptTableRecord
    {
    }

    public class GamesPortalDatabaseUpgrader : DatabaseUpgrader<IGamesPortalDataContext, UpgradeScript>
    {
        protected override string GetUpgradeScriptsResourcePath()
        {
            return "GamesPortal.Service.DataAccessLayer.UpgradeScripts";
        }

        protected override System.Data.Linq.Table<UpgradeScript> GetUpgradeScriptTable(IGamesPortalDataContext dataContext)
        {
            return (System.Data.Linq.Table<UpgradeScript>)dataContext.GetTable<UpgradeScript>();
        }
    }
}
