using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql
{
    public interface IDbDataContext : IDisposable
    {
        System.Data.ConnectionState ConnectionState { get; }
        void OpenConnection();
        void CloseConnection();
        IDbTransaction BeginTransaction();
        void SubmitChanges();
        int ExecuteCommand(string command, params object[] parameters);
        IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters);
        bool HasChanges { get; }

        ChangeSet<object> GetChanges();

        System.Data.Linq.ITable<TEntity> GetTable<TEntity>() where TEntity : class;
    }
}
