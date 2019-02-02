using System;

namespace Tello.Controller
{
    public sealed class DroneStateReceivedArgs : EventArgs
    {
        public DroneStateReceivedArgs(DroneState state)
        {
            State = state;
        }

        public DroneState State { get; }
    }
}
