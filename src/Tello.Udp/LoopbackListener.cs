//using System;
//using System.Net;
//using System.Net.Sockets;
//using System.Text;
//using System.Threading;
//using System.Threading.Tasks;

//namespace Tello.Udp
//{
//    /// <summary>
//    /// This class exists because from UWP applications can't listen on loopback without first establishing a connection to the UDP sender.
//    /// It's a hack to enable UWP apps to connect to a Tello emulator running in a console app on the same machine.
//    /// //https://stackoverflow.com/questions/33259763/uwp-enable-local-network-loopback
//    /// </summary>
//    public sealed class LoopbackListener : IUdpListener, IDisposable
//    {
//        public LoopbackListener(string host, int port) : base()
//        {
//            if (String.IsNullOrEmpty(host))
//            {
//                throw new ArgumentNullException(nameof(host));
//            }

//            _endPoint = new IPEndPoint(IPAddress.Parse(host), port);
//        }

//        private readonly IPEndPoint _endPoint;
//        private UdpClient _client = null;

//        public void Start()
//        {
//            if (!IsActive)
//            {
//                IsActive = true;

//                _client = new UdpClient();
//                _client.Connect(_endPoint);

//                var datagram = Encoding.UTF8.GetBytes("connect");
//                _client.Send(datagram, datagram.Length);
//                _client.BeginReceive(OnReceive, null);
//            }
//        }

//        public void Stop()
//        {
//            if (IsActive)
//            {
//                if (_client != null)
//                {
//                    _client.Close();
//                    _client.Dispose();
//                    _client = null;
//                }
//                IsActive = false;
//            }
//        }

//        public event EventHandler<DatagramReceivedArgs> DatagramReceived;

//        public bool IsActive { get; private set; }
//        public int Port => _endPoint.Port;

//        private void OnReceive(IAsyncResult ar)
//        {
//            if (!_isDisposed && IsActive)
//            {
//                if (_client != null && _client.Client != null)
//                {
//                    var endpoint = new IPEndPoint(IPAddress.Any, 0);
//                    var response = Encoding.UTF8.GetString(_client.EndReceive(ar, ref endpoint));
//                    if (response == "ok")
//                    {
//                        Listen();
//                    }
//                }
//            }
//        }

//        private async void Listen()
//        {
//            await Task.Run(async () =>
//            {
//                var wait = new SpinWait();
//                while (IsActive)
//                {
//                    var receiveResult = await _client.ReceiveAsync();
//                    var eventArgs = new DatagramReceivedArgs(receiveResult.Buffer, receiveResult.RemoteEndPoint);
//                    DatagramReceived?.Invoke(this, eventArgs);
//                    if (eventArgs.Reply != null)
//                    {
//                        await _client.SendAsync(eventArgs.Reply, eventArgs.Reply.Length, eventArgs.RemoteEndpoint);
//                    }
//                    wait.SpinOnce();
//                }
//            });
//        }

//        #region IDisposable Support
//        private bool _isDisposed = false; // To detect redundant calls
//        private void Dispose(bool disposing)
//        {
//            if (!_isDisposed)
//            {
//                if (disposing)
//                {
//                    Stop();
//                }

//                _isDisposed = true;
//            }
//        }

//        // This code added to correctly implement the disposable pattern.
//        public void Dispose()
//        {
//            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
//            Dispose(true);
//        }
//        #endregion
//    }
//}
