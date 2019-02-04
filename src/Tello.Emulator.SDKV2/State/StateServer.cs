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

        public ReceiverStates State { get; private set; }

        public async void Listen(Action<IRelayService<IDroneState>, IDroneState> messageHandler, Action<IRelayService<IDroneState>, Exception> errorHandler)
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
                        messageHandler?.Invoke(this, new DroneState(_droneState));
                    }
                    catch (Exception ex)
                    {
                        errorHandler?.Invoke(this, ex);
                    }
                }
            });
        }

        public void Stop()
        {
            State = ReceiverStates.Stopped;
        }
    }
}
