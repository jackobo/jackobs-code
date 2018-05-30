namespace Spark.Infra.Data.LinqToSql.RecordsSynchronization
{
    public interface IChildRecordsSynchronizer<TParentRecord>
    {
        void Sync(TParentRecord oldParentRecord, TParentRecord newParentRecord);
    }
}
