// <copyright file="SqliteEntity.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Repository.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id}")]
    public class SqliteEntity : IEntity
    {
        public SqliteEntity()
        {
        }

        protected SqliteEntity(IEntity entity)
        {
            this.Id = entity.Id;
        }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        [SQLite.AutoIncrement]
        public int Id { get; set; }
    }
}
