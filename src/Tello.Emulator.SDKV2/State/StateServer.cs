using System;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal sealed class StateServer : IRelayService<IRawDroneState>
    {
        internal StateServer(IRawDroneState droneState)
        {
            _droneState = droneState ?? throw new ArgumentNullException(nameof(droneState));
        }

        private readonly IRawDroneState _droneState;

        public event EventHandler<RelayMessageReceivedArgs<IRawDroneState>> RelayMessageReceived;
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
                            RelayMessageReceived?.Invoke(this, new RelayMessageReceivedArgs<IRawDroneState>(new DroneState(_droneState)));
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
