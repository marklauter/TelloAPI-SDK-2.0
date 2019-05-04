using Repository;
using System;

namespace SqliteRepository.Test
{
    [System.Diagnostics.DebuggerDisplay("{Id}: {Name}, {Timestamp}")]
    public class TestEntity : SqliteEntity
    {
        public TestEntity()
        {
        }

        public TestEntity(TestEntity entity) : base(entity)
        {
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
