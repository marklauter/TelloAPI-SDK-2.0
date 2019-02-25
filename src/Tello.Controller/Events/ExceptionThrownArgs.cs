using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class ExceptionThrownArgs : EventArgs
    {
        public ExceptionThrownArgs(TelloControllerException ex)
        {
            Exception = ex;
        }

        public TelloControllerException Exception { get; }
    }

    public class CommandExceptionThrownArgs : ExceptionThrownArgs
    {
        public CommandExceptionThrownArgs(Commands command, TelloControllerException ex, TimeSpan elapsed) : base(ex)
        {
            Command = command;
            Elapsed = elapsed;
        }

        public Commands Command { get; }
        public TimeSpan Elapsed { get; }
    }
}
