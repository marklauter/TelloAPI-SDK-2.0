using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public sealed class DroneStateReceivedArgs : EventArgs
    {
        public DroneStateReceivedArgs(IDroneState state)
        {
            State = state;
        }

        public IDroneState State { get; }
    }
}
