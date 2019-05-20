using System;

namespace Tello.Events
{
    public sealed class ConnectionStateChangedArgs : EventArgs
    {
        public ConnectionStateChangedArgs(bool isConnected)
        {
            IsConnected = isConnected;
        }

        public bool IsConnected { get; }
    }
}
