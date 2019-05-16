using System;
using Tello.State;

namespace Tello.Events
{
    public sealed class StateChangedArgs : EventArgs
    {
        public StateChangedArgs(ITelloState state)
        {
            State = state ?? throw new ArgumentNullException(nameof(state));
        }

        public ITelloState State { get; }
    }
}
