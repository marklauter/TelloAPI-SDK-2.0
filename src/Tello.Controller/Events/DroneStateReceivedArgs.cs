using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class DroneStateReceivedArgs : EventArgs
    {
        public DroneStateReceivedArgs(IRawDroneState state)
        {
            State = state;
        }

        public IRawDroneState State { get; }
    }
}
