using Repository;

namespace SqliteRepository
{
    public class SqliteEntity : IEntity
    {
        public SqliteEntity() { }

        public SqliteEntity(IEntity entity)
        {
            Id = entity.Id;
        }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        [SQLite.AutoIncrement]
        public int Id { get; set; }
    }
}
