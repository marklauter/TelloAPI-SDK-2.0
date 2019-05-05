using System;

namespace Repository.Sqlite.Test
{
    [System.Diagnostics.DebuggerDisplay("{Id}: {Name}, {Timestamp}")]
    public class TestEntity : SqliteEntity
    {
        public TestEntity()
        {
            Timestamp = DateTime.Now;
            Guid = Guid.NewGuid();
        }

        public TestEntity(TestEntity entity) : base(entity)
        {
            Name = entity.Name;
            Guid = entity.Guid;
            Timestamp = entity.Timestamp;
        }

        public TestEntity(string name)
        {
            Name = name;
            Guid = Guid.NewGuid();
            Timestamp = DateTime.Now;
        }

        public string Name { get; set; }

        public Guid Guid { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
