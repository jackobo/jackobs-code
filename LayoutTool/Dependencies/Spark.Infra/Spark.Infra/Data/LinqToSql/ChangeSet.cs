using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Spark.Infra.Data.LinqToSql
{
    public class ChangeSet<TEntity>
    {
        public ChangeSet(IEnumerable<TEntity> inserted, IEnumerable<TEntity> updated, IEnumerable<TEntity> deleted)
        {
            Inserted = inserted ?? new TEntity[0];
            Updated = updated ?? new TEntity[0];
            Deleted = deleted ?? new TEntity[0];
        }

        public IEnumerable<TEntity> Inserted { get; private set; }
        public IEnumerable<TEntity> Updated { get; private set; }
        public IEnumerable<TEntity> Deleted { get; private set; }

        public static ChangeSet<TEntity> CreateInserted(params TEntity[] entities)
        {
            return new ChangeSet<TEntity>(entities, null, null);
        }

        public static ChangeSet<TEntity> CreateUpdated(params TEntity[] entities)
        {
            return new ChangeSet<TEntity>(null, entities, null);
        }


        public static ChangeSet<TEntity> CreateDeleted(params TEntity[] entities)
        {
            return new ChangeSet<TEntity>(null, null, entities);
        }

        public bool IsEmpty()
        {
            return !Inserted.Any() && !Updated.Any() && !Deleted.Any();
        }

        public IEnumerable<TEntity> GetAll()
        {
            return Inserted.Concat(Updated).Concat(Deleted);
        }

        public ChangeSet<TFilteredEntity> Filter<TFilteredEntity>()
        {
            return new ChangeSet<TFilteredEntity>(FilterByType<TFilteredEntity>(Inserted),
                                                  FilterByType<TFilteredEntity>(Updated),
                                                  FilterByType<TFilteredEntity>(Deleted));
        }

        private IEnumerable<TFilteredEntity> FilterByType<TFilteredEntity>(IEnumerable<TEntity> items)
        {
            return items.Where(item => typeof(TFilteredEntity).Equals(item.GetType())).Cast<TFilteredEntity>();
        }

        public static ChangeSet<object> Empty()
        {
            return new ChangeSet<object>(new object[0], new object[0], new object[0]);
        }
    }
}
