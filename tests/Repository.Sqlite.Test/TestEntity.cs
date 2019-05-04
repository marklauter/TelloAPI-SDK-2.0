using System;

namespace Repository.Sqlite.Test
{
    [System.Diagnostics.DebuggerDisplay("{Id}: {Name}, {Timestamp}")]
    public class TestEntity : SqliteEntity
    {
        public TestEntity()
        {
            Timestamp = DateTime.Now;
        }

        public TestEntity(TestEntity entity) : base(entity)
        {
            Name = entity.Name;
            Timestamp = entity.Timestamp;
        }

        public TestEntity(string name)
        {
            Name = name;
            Timestamp = DateTime.Now;
        }

        public string Name { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
