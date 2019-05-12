using System;

namespace Tello.Controller.Events
{
    public sealed class ConnectionStateChangedArgs : EventArgs
    {
        public ConnectionStateChangedArgs(bool connected)
        {
            Connected = connected;
        }

        public bool Connected { get; }
    }
}
