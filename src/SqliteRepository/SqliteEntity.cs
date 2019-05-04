namespace Repository.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id}")]
    public class SqliteEntity : IEntity
    {
        public SqliteEntity() { }

        protected SqliteEntity(IEntity entity)
        {
            Id = entity.Id;
        }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        [SQLite.AutoIncrement]
        public int Id { get; set; }
    }
}
