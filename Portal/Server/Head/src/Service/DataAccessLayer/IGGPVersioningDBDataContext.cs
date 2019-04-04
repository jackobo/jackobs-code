using System;
using Spark.Infra.Data.LinqToSql;
using System.Linq;

namespace GamesPortal.Service.GGPVersioning
{
    public interface IGGPVersioningDBDataContext : IDbDataContext
    {
        
    }

    public partial class GGPVersioningDBDataContext : IGGPVersioningDBDataContext
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
