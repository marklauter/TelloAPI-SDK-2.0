// <copyright file="TestEntity.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Repository.Sqlite.Test
{
    [System.Diagnostics.DebuggerDisplay("{Id}: {Name}, {Timestamp}")]
    public class TestEntity : SqliteEntity
    {
        public TestEntity()
        {
            this.Timestamp = DateTime.Now;
            this.Guid = Guid.NewGuid();
        }

        public TestEntity(TestEntity entity)
            : base(entity)
        {
            this.Name = entity.Name;
            this.Guid = entity.Guid;
            this.Timestamp = entity.Timestamp;
        }

        public TestEntity(string name)
        {
            this.Name = name;
            this.Guid = Guid.NewGuid();
            this.Timestamp = DateTime.Now;
        }

        public string Name { get; set; }

        public Guid Guid { get; set; }

        public DateTime Timestamp { get; set; }
    }
}
