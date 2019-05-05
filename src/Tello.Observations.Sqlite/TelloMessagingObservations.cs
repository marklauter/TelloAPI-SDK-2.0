using System;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.Observations
{
    public sealed class TelloCommandObservation : Observation, ICommandResponseReceivedArgs
    {
        public TelloCommandObservation() : base() { }

        public TelloCommandObservation(ICommandResponseReceivedArgs commandArgs, string groupId) : base(groupId)
        {
            if (commandArgs == null)
            {
                throw new ArgumentNullException(nameof(commandArgs));
            }
            Command = commandArgs.Command;
            Response = commandArgs.Response;
            Initiated = commandArgs.Initiated;
            Elapsed = commandArgs.Elapsed;
            Completed = Initiated + Elapsed;
        }

        [SQLite.Ignore]
        public TelloCommands Command { get; set; }

        [SQLite.Column("Command")]
        public string CommandValue
        {
            get => Command.ToString();
            set => Command = (TelloCommands)Enum.Parse(typeof(TelloCommands), value);
        }

        public string Response { get; set; }

        public DateTime Initiated { get; set; }
        // this is just to put a human readable value in sqlite for debugging
        public string InitiatedString
        {
            get => Initiated.ToString("o");
            set { }
        }

        public DateTime Completed { get; set; }
        // this is just to put a human readable value in sqlite for debugging
        public string CompletedString
        {
            get => Completed.ToString("o");
            set { }
        }

        [SQLite.Ignore]
        public TimeSpan Elapsed { get; set; }
        public int ElapsedMS
        {
            get => (int)Elapsed.TotalMilliseconds;
            set => Elapsed = TimeSpan.FromMilliseconds(value);
        }
    }
}
