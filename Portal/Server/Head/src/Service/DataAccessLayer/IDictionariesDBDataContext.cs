using System;
using Spark.Infra.Data.LinqToSql;
using System.Linq;
using Spark.Infra.Types;
using Spark.Infra.Exceptions;

namespace GamesPortal.Service.SDM
{
    public interface ISdmDataContext : IDbDataContext
    {   
        int GetRegulationIdForBrand(int brandId);
    }


    public partial class SdmDataContext : ISdmDataContext
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


        public int GetRegulationIdForBrand(int brandId)
        {
            var brand = GetTable<SDM.Brand>().FirstOrDefault(b => b.BRN_ID == brandId);

            if (brand == null)
            {
                throw new ArgumentException($"Cannot find the regulation type ID for brand {brandId}", nameof(brandId));
            }


            if (brand.BRN_RGLT_ID == null)
            {
                throw new ValidationException($"There is no regulation type defined for brand {brandId}");
            }

            return brand.BRN_RGLT_ID.Value;
        }
    }
}
