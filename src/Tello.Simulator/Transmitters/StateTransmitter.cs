using System.Text;
using Tello.State;

namespace Tello.Simulator.Transmitters
{
    //todo: when the drone sim does stuff, it will report it's state to the StateTransmitterSim
    public sealed class StateTransmitter : Transmitter
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
