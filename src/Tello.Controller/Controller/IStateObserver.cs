using Messenger;
using System;
using Tello.Controller.Events;
using Tello.State;

namespace Tello.Controller
{
    public interface IStateObserver : IObserver<IEnvelope>
    {
        event EventHandler<StateChangedArgs> StateChanged;

        void UpdatePosition(object sender, PositionChangedArgs e);

        ITelloState State { get; }
    }
}
