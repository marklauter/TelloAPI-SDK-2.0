using System;
using Tello.Messaging;

namespace Tello.Controller
{
    public class TelloStateChangedArgs : EventArgs
    {
        public TelloStateChangedArgs(ITelloState state)
        {
            State = state;
        }

        public ITelloState State { get; }
    }
}
