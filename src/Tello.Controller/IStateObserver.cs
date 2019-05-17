using Messenger;
using System;
using Tello.Events;
using Tello.State;

namespace Tello.Controller
{
    public interface IStateObserver : IObserver<IEnvelope>
    {
        event EventHandler<StateChangedArgs> StateChanged;
        event EventHandler<Exception> ExceptionThrown;

        void UpdatePosition(object sender, PositionChangedArgs e);

        ITelloState State { get; }
    }
}
