using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public interface ICommandResponseReceivedArgs
    {
        TelloCommands Command { get; }
        string Response { get; }
        DateTime Initiated { get; }
        TimeSpan Elapsed { get; }
    }

    public class CommandResponseReceivedArgs : EventArgs, ICommandResponseReceivedArgs
    {
        public CommandResponseReceivedArgs(TelloCommands command, string response, DateTime initiated, TimeSpan elapsed)
        {
            Command = command;
            Response = response;
            Initiated = initiated;
            Elapsed = elapsed;
        }

        public TelloCommands Command { get; }
        public string Response { get; }
        public DateTime Initiated { get; }
        public TimeSpan Elapsed { get; }
    }
}
