using System;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal sealed class StateServer : IRelayService<IDroneState>
    {
        internal StateServer(IDroneState droneState)
        {
            _droneState = droneState ?? throw new ArgumentNullException(nameof(droneState));
        }

        private readonly IDroneState _droneState;

        public event EventHandler<RelayMessageReceivedArgs<IDroneState>> RelayMessageReceived;
        public event EventHandler<RelayExceptionThrownArgs> RelayExceptionThrown;

        public ReceiverStates State { get; private set; }

        public async void Start()
        {
            if (State != ReceiverStates.Listening)
            {
                State = ReceiverStates.Listening;

                await Task.Run(async () =>
                {
                    while (State == ReceiverStates.Listening)
                    {
                        // 5Hz state reporting
                        await Task.Delay(200);
                        try
                        {
                            RelayMessageReceived?.Invoke(this, new RelayMessageReceivedArgs<IDroneState>(new DroneState(_droneState)));
                        }
                        catch (Exception ex)
                        {
                            RelayExceptionThrown?.Invoke(this, new RelayExceptionThrownArgs(ex));
                        }
                    }
                });
            }
        }

        public void Stop()
        {
            State = ReceiverStates.Stopped;
        }
    }
}
