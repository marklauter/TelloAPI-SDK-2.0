using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Udp
{
    public sealed class UdpReceiver : Receiver
    {
        private readonly int _port;

        public UdpReceiver(int port)
        {
            _port = port;
        }

        protected override async Task Listen(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            var wait = new SpinWait();

            using (var client = new UdpClient(_port))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (client.Available > 0)
                        {
                            MessageReceived(new Envelope((await client.ReceiveAsync()).Buffer));
                        }
                        else
                        {
                            wait.SpinOnce();
                        }
                    }
                    catch (Exception ex)
                    {
                        ExceptionThrown(ex);
                    }
                }
            }
        }
    }
}
