using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class CommandResponseReceivedArgs : EventArgs
    {
        public CommandResponseReceivedArgs(Commands command, string response, TimeSpan elapsed)
        {
            Command = command;
            Response = response;
            Elapsed = elapsed;
        }

        public Commands Command { get; }
        public string Response { get; }
        public TimeSpan Elapsed { get; }
    }
}
