using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Spark.Infra.Data.LinqToSql;

namespace GamesPortal.Service.DataAccessLayer
{
    public interface IJackpotInfoDBDataContext : IDbDataContext
    {
    }
}

namespace GamesPortal.Service.Jackpot
{
    using GamesPortal.Service.DataAccessLayer;
    public partial class JackpotInfoDBDataContext : IJackpotInfoDBDataContext
    {

        private DbDataContextImplementationHelper Helper
        {
            get { return new DbDataContextImplementationHelper(this); }
        }
        #region IDbDataContext Members

        System.Data.ConnectionState IDbDataContext.ConnectionState
        {
            get { return Helper.ConnectionState; }
        }

        void IDbDataContext.OpenConnection()
        {
            Helper.OpenConnection();
        }

        void IDbDataContext.CloseConnection()
        {
            Helper.CloseConnection();

        }

        System.Data.IDbTransaction IDbDataContext.BeginTransaction()
        {
            return Helper.BeginTransaction();
        }


        System.Data.Linq.ITable<TEntity> IDbDataContext.GetTable<TEntity>()
        {
            return Helper.GetTable<TEntity>();
        }


        bool IDbDataContext.HasChanges
        {
            get
            {

                return Helper.HasChanges;
            }
        }

        ChangeSet<object> IDbDataContext.GetChanges()
        {
            return Helper.GetChanges();
        }

        #endregion

    }
}
