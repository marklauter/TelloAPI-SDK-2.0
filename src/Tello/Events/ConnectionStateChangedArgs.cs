using System;

namespace Tello.Events
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
