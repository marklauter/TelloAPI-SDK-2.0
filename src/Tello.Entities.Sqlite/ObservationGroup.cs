// <copyright file="ObservationGroup.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Repository.Sqlite;

namespace Tello.Entities.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id}:{SessionId} {Timestamp}")]
    public sealed class ObservationGroup : SqliteEntity, IObservationGroup
    {
        public ObservationGroup()
            : this(0, DateTime.UtcNow)
        {
        }

        public ObservationGroup(int sessionId)
            : this(sessionId, DateTime.UtcNow)
        {
        }

        public ObservationGroup(ISession session)
            : this(session.Id, DateTime.UtcNow)
        {
        }

        public ObservationGroup(ISession session, DateTime timestamp)
            : this(session.Id, timestamp)
        {
        }

        public ObservationGroup(int sessionId, DateTime timestamp)
        {
            this.SessionId = sessionId;
            this.Timestamp = timestamp;
        }

        [SQLite.Indexed]
        public int SessionId { get; set; }

        [SQLite.Indexed]
        public DateTime Timestamp { get; set; }
    }
}
