using System;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal sealed class StateServer : IMessageRelayService<IRawDroneState>
    {
        internal StateServer(IRawDroneState droneState)
        {
            _droneState = droneState ?? throw new ArgumentNullException(nameof(droneState));
        }

        private readonly IRawDroneState _droneState;

        public event EventHandler<MessageReceivedArgs<IRawDroneState>> MessageReceived;
        public event EventHandler<MessageRelayExceptionThrownArgs> MessageRelayExceptionThrown;

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
                            MessageReceived?.Invoke(this, new MessageReceivedArgs<IRawDroneState>(new RawDroneStateEmulated(_droneState)));
                        }
                        catch (Exception ex)
                        {
                            MessageRelayExceptionThrown?.Invoke(this, new MessageRelayExceptionThrownArgs(ex));
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
