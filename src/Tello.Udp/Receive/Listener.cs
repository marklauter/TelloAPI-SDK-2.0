using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Tello.Udp
{
    public sealed class Listener : IUdpListener, IDisposable
    {
        public Listener(int port) : base()
        {
            Port = port;
        }

        public event EventHandler<DatagramReceivedArgs> DatagramReceived;

        public int Port { get; }
        private volatile bool _isActive = false;
        public bool IsActive => _isActive;

        public void Start()
        {
            if (!_isActive)
            {
                _isActive = true;
                Listen();
            }
        }

        public void Stop()
        {
            _isActive = false;
        }

        private async void Listen()
        {
            await Task.Run(async () =>
            {
                using (var client = new UdpClient(Port))
                {
                    while (_isActive)
                    {
                        var receiveResult = await client.ReceiveAsync();
                        var eventArgs = new DatagramReceivedArgs(receiveResult.Buffer, receiveResult.RemoteEndPoint);
                        DatagramReceived?.Invoke(this, eventArgs);
                        if (eventArgs.Reply != null)
                        {
                            await client.SendAsync(eventArgs.Reply, eventArgs.Reply.Length, eventArgs.RemoteEndpoint);
                        }
                    }
                }
            });
        }

        #region IDisposable Support
        private bool _isDisposed = false; // To detect redundant calls
        private void Dispose(bool disposing)
        {
            if (!_isDisposed)
            {
                if (disposing)
                {
                    Stop();
                }

                _isDisposed = true;
            }
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
        }
        #endregion

    }
}
