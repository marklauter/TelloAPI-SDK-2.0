using Repository.Sqlite;
using System;

namespace Tello.Observations.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id} {Start} {Duration}")]
    public sealed class Session : SqliteEntity, ISession
    {
        public Session() : this(DateTime.UtcNow) { }

        public Session(DateTime start) : this(start, TimeSpan.FromSeconds(0)) { }

        public Session(DateTime start, TimeSpan duration)
        {
            Start = start;
            Duration = duration;
        }

        [SQLite.Indexed]
        public DateTime Start { get; set; }

        [SQLite.Ignore]
        public TimeSpan Duration { get; set; }

        [SQLite.Indexed]
        public int ElapsedMs
        {
            get => (int)Duration.TotalMilliseconds;
            set => Duration = TimeSpan.FromMilliseconds(value);
        }
    }
}
