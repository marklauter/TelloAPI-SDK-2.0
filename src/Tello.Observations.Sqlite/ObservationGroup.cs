using System;

namespace Tello.Observations.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id}:{SessionId} {Timestamp}")]
    public sealed class ObservationGroup : IObservationGroup
    {
        public ObservationGroup() : this(0, DateTime.UtcNow) { }

        public ObservationGroup(int sessionId) : this(sessionId, DateTime.UtcNow) { }

        public ObservationGroup(ISession session) : this(session.Id, DateTime.UtcNow) { }

        public ObservationGroup(ISession session, DateTime timestamp) : this(session.Id, timestamp) { }

        public ObservationGroup(int sessionId, DateTime timestamp)
        {
            SessionId = sessionId;
            Timestamp = timestamp;
        }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        [SQLite.AutoIncrement]
        public int Id { get; set; }

        [SQLite.Indexed]
        public int SessionId { get; set; }

        [SQLite.Indexed]
        public DateTime Timestamp { get; set; }
    }
}
