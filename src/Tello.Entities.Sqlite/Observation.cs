// <copyright file="Observation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Repository.Sqlite;

namespace Tello.Entities.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id}:{GroupId} {Timestamp}")]
    public abstract class Observation : SqliteEntity, IObservation
    {
        public Observation()
            : this(0)
        {
        }

        public Observation(IObservationGroup group)
            : this((group ?? throw new ArgumentNullException(nameof(group))).Id)
        {
        }

        public Observation(int groupId)
            : this(
                  groupId,
                  DateTime.UtcNow)
        {
        }

        public Observation(
            int groupId,
            DateTime timestamp)
            : base()
        {
            this.GroupId = groupId;
            this.Timestamp = timestamp;
        }

        public Observation(IObservation observation)
            : base(observation)
        {
            this.GroupId = observation.GroupId;
            this.Timestamp = observation.Timestamp;
        }

        [SQLite.Indexed]
        public int GroupId { get; set; }

        [SQLite.Indexed]
        public DateTime Timestamp { get; set; }

        // this is just to put a human readable value in sqlite for debugging
        public string TimestampString
        {
            get => this.Timestamp.ToString("o");
            set { }
        }
    }
}
