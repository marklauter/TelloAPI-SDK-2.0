using System;
using Tello.State;

namespace Tello.Controller.Events
{
    public sealed class PositionChangedArgs : EventArgs
    {
        public PositionChangedArgs(Vector position)
        {
            Position = position;
        }

        public Vector Position { get; }
    }
}
