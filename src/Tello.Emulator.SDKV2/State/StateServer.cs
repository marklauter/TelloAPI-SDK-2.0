using System;
using System.Text;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal sealed class StateServer : IRelay<INotification>
    {
        public StateServer(IDroneState droneState)
        {
            _droneState = droneState ?? throw new System.ArgumentNullException(nameof(droneState));
        }

        private readonly IDroneState _droneState;

        public ReceiverStates State { get; private set; }

        public async void Listen(Action<IRelay<INotification>, INotification> messageHandler, Action<IRelay<INotification>, Exception> errorHandler)
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
                        var bytes = Encoding.UTF8.GetBytes(_droneState.ToString());
                        messageHandler?.Invoke(this, Notification.FromData(bytes));
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
