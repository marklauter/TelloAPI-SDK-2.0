using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class UdpReceiver : IMessageRelayService<DataNotification>
    {
        internal UdpReceiver(int port)
        {
            _port = port;
        }

        private readonly int _port;

        public event EventHandler<MessageReceivedArgs<DataNotification>> MessageReceived;
        public event EventHandler<MessageRelayExceptionThrownArgs> MessageRelayExceptionThrown;

        public ReceiverStates State { get; private set; }

        public async void Start()
        {
            if (State != ReceiverStates.Listening)
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
                                    var message = DataNotification.FromData(receiveResult.Buffer);
                                    MessageReceived?.Invoke(this, new MessageReceivedArgs<DataNotification>(message));
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
                                MessageRelayExceptionThrown?.Invoke(this, new MessageRelayExceptionThrownArgs(ex));
                                wait.SpinOnce();
                            }
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
