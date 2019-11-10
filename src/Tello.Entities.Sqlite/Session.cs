// <copyright file="Session.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Repository.Sqlite;

namespace Tello.Entities.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id} {Start} {Duration}")]
    public sealed class Session : SqliteEntity, ISession
    {
        public Session()
            : this(DateTime.UtcNow)
        {
        }

        public Session(DateTime start)
            : this(start, TimeSpan.FromSeconds(0))
        {
        }

        public Session(DateTime start, TimeSpan duration)
        {
            this.StartTime = start;
            this.Duration = duration;
        }

        [SQLite.Indexed]
        public DateTime StartTime { get; set; }

        [SQLite.Ignore]
        public TimeSpan Duration { get; set; }

        [SQLite.Indexed]
        public int ElapsedMs
        {
            get => (int)this.Duration.TotalMilliseconds;
            set => this.Duration = TimeSpan.FromMilliseconds(value);
        }
    }
}
