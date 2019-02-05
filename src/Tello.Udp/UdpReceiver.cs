using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Udp
{
    internal sealed class UdpReceiver : IRelayService<DataNotification>
    {
        internal UdpReceiver(int port)
        {
            _port = port;
        }

        private readonly int _port;

        public event EventHandler<RelayMessageReceivedArgs<DataNotification>> RelayMessageReceived;
        public event EventHandler<RelayExceptionThrownArgs> RelayExceptionThrown;

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
                                    RelayMessageReceived?.Invoke(this, new RelayMessageReceivedArgs<DataNotification>(message));
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
                                RelayExceptionThrown?.Invoke(this, new RelayExceptionThrownArgs(ex));
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
