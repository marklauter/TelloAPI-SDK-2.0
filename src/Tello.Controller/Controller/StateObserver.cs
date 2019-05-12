using Messenger;
using System;
using System.Text;
using Tello.Controller.Events;
using Tello.State;

namespace Tello.Controller
{
    internal sealed class StateObserver : Observer<IEnvelope>
    {
        public StateObserver(IReceiver receiver) : base(receiver)
        {
        }

        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnNext(IEnvelope message)
        {
            State = new TelloState(Encoding.UTF8.GetString(message.Data), message.Timestamp);
            StateChanged?.Invoke(this, new StateChangedArgs(State));
        }

        public ITelloState State { get; private set; }

        public event EventHandler<StateChangedArgs> StateChanged;
    }
}
