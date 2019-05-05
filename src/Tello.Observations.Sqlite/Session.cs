using System;

namespace Tello.Observations.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id} {Start} {Duration}")]
    public sealed class Session : ISession
    {
        public Session() : this(DateTime.UtcNow) { }

        public Session(DateTime start) : this(start, TimeSpan.FromSeconds(0)) { }

        public Session(DateTime start, TimeSpan duration)
        {
            Start = start;
            Duration = duration;
        }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        [SQLite.AutoIncrement]
        public int Id { get; set; }

        [SQLite.Indexed]
        public DateTime Start { get; set; }

        [SQLite.Indexed]
        public TimeSpan Duration { get; set; }
    }
}
