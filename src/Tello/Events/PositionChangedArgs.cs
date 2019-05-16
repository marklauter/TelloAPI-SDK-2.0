using System;
using Tello.State;

namespace Tello.Events
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
