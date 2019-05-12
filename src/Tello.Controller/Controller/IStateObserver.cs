using Messenger;
using System;
using Tello.Controller.Events;
using Tello.State;

namespace Tello.Controller
{
    public interface IStateObserver : IObserver<IEnvelope>
    {
        event EventHandler<StateChangedArgs> StateChanged;

        ITelloState State { get; }
    }
}
