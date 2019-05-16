using System.Text;
using Tello.State;

namespace Tello.Simulator.Messaging
{
    internal sealed class StateTransmitter : Transmitter
    {
        public void SetState(ITelloState telloState)
        {
            if (telloState != null)
            {
                _available = 0;
                Buffer = Encoding.UTF8.GetBytes(telloState.ToString());
            }
        }
    }
}
