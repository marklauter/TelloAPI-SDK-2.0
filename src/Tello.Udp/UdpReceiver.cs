using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class UdpReceiver : IRelayService<INotification>
    {
        internal UdpReceiver(int port)
        {
            _port = port;
        }

        private readonly int _port;

        public ReceiverStates State { get; private set; }

        public async void Listen(Action<IRelayService<INotification>, INotification> messageHandler, Action<IRelayService<INotification>, Exception> errorHandler)
        {
            State = ReceiverStates.Listening;

            await Task.Run(async () =>
            {
                var wait = new SpinWait();
                using (var client = new UdpClient(_port))
                {
                    while (State == ReceiverStates.Listening)
                    {
                        try
                        {
                            if (client.Available > 0)
                            {
                                var receiveResult = await client.ReceiveAsync();
                                var message = Notification.FromData(receiveResult.Buffer);
                                messageHandler?.Invoke(this, message);
                                if (message.Reply != null)
                                {
                                    await client.SendAsync(message.Reply, message.Reply.Length, receiveResult.RemoteEndPoint);
                                }
                            }
                            else
                            {
                                wait.SpinOnce();
                            }
                        }
                        catch (Exception ex)
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
