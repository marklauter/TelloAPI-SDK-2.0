using System;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.Observations.Sqlite
{
    public sealed class CommandObservation : Observation, ICommandObservation
    {
        public CommandObservation() : base() { }

        public CommandObservation(IObservationGroup group, ICommandResponseReceivedArgs commandArgs)
            : this(group.Id, commandArgs) { }

        public CommandObservation(int groupId, ICommandResponseReceivedArgs commandArgs) 
            : base(groupId)
        {
            Command = commandArgs.Command;
            Response = commandArgs.Response;
            Timestamp = Initiated = commandArgs.Initiated;
            Elapsed = commandArgs.Elapsed;
            Completed = Initiated + Elapsed;
        }

        [SQLite.Ignore]
        public TelloCommands Command { get; set; }

        [SQLite.Indexed]
        [SQLite.Column("Command")]
        public string CommandValue
        {
            get => Command.ToString();
            set => Command = (TelloCommands)Enum.Parse(typeof(TelloCommands), value);
        }

        [SQLite.Indexed]
        public string Response { get; set; }

        [SQLite.Indexed]
        public DateTime Initiated { get; set; }
        
        // this is just to put a human readable value in sqlite for debugging
        public string InitiatedString
        {
            get => Initiated.ToString("o");
            set { }
        }

        [SQLite.Indexed]
        public DateTime Completed { get; set; }

        // this is just to put a human readable value in sqlite for debugging
        public string CompletedString
        {
            get => Completed.ToString("o");
            set { }
        }

        [SQLite.Ignore]
        public TimeSpan Elapsed { get; set; }

        [SQLite.Indexed]
        public int ElapsedMs
        {
            get => (int)Elapsed.TotalMilliseconds;
            set => Elapsed = TimeSpan.FromMilliseconds(value);
        }
    }
}
