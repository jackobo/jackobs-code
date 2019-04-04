using System;
using Spark.Infra.Data.LinqToSql;
using System.Linq;
using System.Collections.Generic;

namespace GamesPortal.Service.DataAccessLayer
{
    public interface IGamesPortalDataContext : IDbDataContext
    {
        int NormalizeApprovalStatusForAllLanguagesWithTheSameHash();
    }


    public partial class GamesPortalDataContext : IGamesPortalDataContext
    {

        private DbDataContextImplementationHelper Helper
        {
            get { return new DbDataContextImplementationHelper(this);}
        }
        #region IDbDataContext Members

        System.Data.ConnectionState IDbDataContext.ConnectionState
        {
            get { return Helper.ConnectionState;}
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
