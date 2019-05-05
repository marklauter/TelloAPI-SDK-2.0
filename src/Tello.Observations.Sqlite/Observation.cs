using System;

namespace Tello.Observations
{
    public abstract class Observation : IObservation
    {
        public Observation() { }

        public Observation(string groupId) 
        {
            GroupId = groupId;
            Timestamp = DateTime.UtcNow;
        }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        [SQLite.AutoIncrement]
        public int Id { get; set; }

        [SQLite.Indexed]
        public string GroupId { get; set; }

        [SQLite.Indexed]
        public string SessionId { get; set; }

        [SQLite.Indexed]
        public DateTime Timestamp { get; set; }

        // this is just to put a human readable value in sqlite for debugging
        public string TimestampString
        {
            get => Timestamp.ToString("o");
            set { }
        }
    }
}
