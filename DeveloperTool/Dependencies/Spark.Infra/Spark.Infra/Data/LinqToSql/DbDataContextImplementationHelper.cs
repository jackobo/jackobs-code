using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql
{
    public class DbDataContextImplementationHelper : IDbDataContext
    {
        public DbDataContextImplementationHelper(System.Data.Linq.DataContext linqContext)
        {
            _linqContext = linqContext;
        }

        System.Data.Linq.DataContext _linqContext;

        public ConnectionState ConnectionState
        {
            get
            {
                return _linqContext.Connection.State;
            }
        }

        public bool HasChanges
        {
            get
            {
                var changeSet = _linqContext.GetChangeSet();

                return changeSet.Deletes.Any()
                        || changeSet.Inserts.Any()
                        || changeSet.Updates.Any();
            }
        }

        public void OpenConnection()
        {
            _linqContext.Connection.Open();
        }

        public void CloseConnection()
        {
            _linqContext.Connection.Close();
        }

        public IDbTransaction BeginTransaction()
        {
            var tx = _linqContext.Connection.BeginTransaction();
            _linqContext.Transaction = tx;
            return tx;
        }

        public void SubmitChanges()
        {
            _linqContext.SubmitChanges();
        }

        public int ExecuteCommand(string command, params object[] parameters)
        {
            return _linqContext.ExecuteCommand(command, parameters);
        }

        public IEnumerable<TResult> ExecuteQuery<TResult>(string query, params object[] parameters)
        {
            return _linqContext.ExecuteQuery<TResult>(query, parameters);
        }

        public ChangeSet<object> GetChanges()
        {
            var changeSet = _linqContext.GetChangeSet();
            return new ChangeSet<object>(changeSet.Inserts, changeSet.Updates, changeSet.Deletes);
        }

        public ITable<TEntity> GetTable<TEntity>() where TEntity : class
        {
            return _linqContext.GetTable<TEntity>();
        }

        public void Dispose()
        {
            _linqContext.Dispose();
        }
    }
}
