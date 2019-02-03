using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Udp
{
    public sealed class Receiver : IReceiver<INotification>
    {
        public Receiver(int port) 
        {
            _port = port;
        }

        private readonly int _port;

        public ReceiverStates State { get; private set; }

        public async void Listen(Action<IReceiver<INotification>, INotification> messageHandler, Action<IReceiver<INotification>, Exception> errorHandler)
        {
            State = ReceiverStates.Listening;

            await Task.Run(async () =>
            {
                using (var client = new UdpClient(_port))
                {
                    while (State == ReceiverStates.Listening)
                    {
                        try
                        {
                            var receiveResult = await client.ReceiveAsync();
                            var message = Notification.FromData(receiveResult.Buffer);
                            messageHandler?.Invoke(this, message);
                            if (message.Reply != null)
                            {
                                await client.SendAsync(message.Reply, message.Reply.Length, receiveResult.RemoteEndPoint);
                            }
                        }
                        catch(Exception ex)
                        {
                            errorHandler?.Invoke(this, ex);
                        }
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
