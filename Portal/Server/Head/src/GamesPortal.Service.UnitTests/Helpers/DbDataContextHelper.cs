using System.Data.Linq;
using System.Linq;
using Spark.Infra.Data.LinqToSql;
using NSubstitute;


namespace GamesPortal.Service.Helpers
{
    public static class DbDataContextHelper
    {
        public static ITable<TEntity> MockTable<TDataContext, TEntity>(this TDataContext dataContext, params TEntity[] records)
            where TDataContext : IDbDataContext
            where TEntity : class
        {
            var table = MockTable(records);
            dataContext.GetTable<TEntity>().Returns(table);
            return table;
        }

        private static ITable<TEntity> MockTable<TEntity>(params TEntity[] records) where TEntity : class
        {
            var table = Substitute.For<ITable<TEntity>>();

            FillTable(table, records);

            return table;
        }
                

        private static void FillTable<TEntity>(ITable<TEntity> table, params TEntity[] records) where TEntity : class
        {
            var list = records.ToList();
            table.GetEnumerator().Returns(list.GetEnumerator());
            table.ElementType.Returns(typeof(TEntity));
            table.Provider.Returns(list.AsQueryable().Provider);
            table.Expression.Returns(list.AsQueryable().Expression);
        }

        
     
    }
}
