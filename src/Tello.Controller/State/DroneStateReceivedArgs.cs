using System;

namespace Tello.Controller.State
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
