using Messenger;
using System;
using System.Text;
using Tello.Controller.Events;
using Tello.State;

namespace Tello.Controller
{
    public sealed class StateObserver : Observer<IEnvelope>, IStateObserver
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
            State = new TelloState(Encoding.UTF8.GetString(message.Data), message.Timestamp, _position);
            StateChanged?.Invoke(this, new StateChangedArgs(State));
        }

        public void UpdatePosition(object sender, PositionChangedArgs e)
        {
            _position = e.Position;
        }

        private Vector _position = new Vector();

        public ITelloState State { get; private set; }

        public event EventHandler<StateChangedArgs> StateChanged;
    }
}
