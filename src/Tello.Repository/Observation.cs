using System;

namespace Tello.Repository
{
    public interface IObservation
    {
        [SQLite.Indexed]
        DateTime Timestamp { get; set; }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        string Id { get; set; }

        [SQLite.Indexed]
        string SessionId { get; set; }
    }

    public abstract class Observation : IObservation
    {
        public Observation() { }

        public Observation(bool initialize)
        {
            Timestamp = DateTime.UtcNow;
            Id = Guid.NewGuid().ToString();
        }

        public Observation(string groupId) : this(true)
        {
            GroupId = groupId;
        }

        [SQLite.Indexed]
        [SQLite.PrimaryKey]
        public string Id { get; set; }

        [SQLite.Indexed]
        public string GroupId { get; set; }

        [SQLite.Indexed]
        public DateTime Timestamp { get; set; }

        // this is just to put a human readable value in sqlite for debugging
        public string TimestampString
        {
            get => Timestamp.ToString("o");
            set { }
        }

        public string SessionId { get; set; }
    }
}
