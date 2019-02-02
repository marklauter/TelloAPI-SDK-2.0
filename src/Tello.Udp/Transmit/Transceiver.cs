using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tello.Udp
{
    public sealed class Transceiver : IDisposable
    {
        public Transceiver(string ip, int port, TimeSpan responseTimeout) : base()
        {
            if (String.IsNullOrEmpty(ip))
            {
                throw new ArgumentNullException(nameof(ip));
            }

            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
            Destination = $"{ip}:{port}";

            _responseTimeout = responseTimeout;
        }

        private readonly IPEndPoint _endPoint;
        private readonly TimeSpan _responseTimeout;
        private UdpClient _client = null;

        public event EventHandler Connected;
        public event EventHandler Disconnected;
        public string Destination { get; }

        public void Connect()
        {
            if (!IsConnected)
            {
                IsConnected = true;
                try
                {
                    if (!IsNetworkAvailable)
                    {
                        Debug.WriteLine("NetworkUnavailableException");
                        throw new NetworkUnavailableException("Device must be connected to Tello's WIFI.");
                    }

                    _client?.Dispose();
                    _client = new UdpClient();
                    _client.Connect(_endPoint);

                    Connected?.Invoke(this, EventArgs.Empty);
                }
                catch (Exception ex)
                {
                    IsConnected = false;
                    Debug.WriteLine(ex);
                    throw;
                }
            }
        }

        public void Disconnect()
        {
            if (IsConnected)
            {
                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                    _client = null;
                }
                IsConnected = false;
                Disconnected?.Invoke(this, EventArgs.Empty);
            }
        }

        public bool IsNetworkAvailable => NetworkInterface.GetIsNetworkAvailable();
        public bool IsConnected { get; private set; }

        public class Response
        {
            private Response() { }

            internal static Response FromFailure(string message)
            {
                return new Response
                {
                    IsSuccess = false,
                    ErrorMessage = message
                };
            }

            internal static Response FromException(Exception ex, long elapsedMS)
            {
                return new Response
                {
                    IsSuccess = false,
                    ErrorMessage = $"{ex.GetType().Name} - {ex.Message}",
                    Exception = ex,
                    ElapsedMS = elapsedMS
                };
            }

            internal static Response FromUdpReceiveResult(UdpReceiveResult result, long elapsedMS)
            {
                return new Response
                {
                    IsSuccess = true,
                    ErrorMessage = "ok",
                    Datagram = result.Buffer,
                    Value = result.Buffer != null
                        ? Encoding.UTF8.GetString(result.Buffer)
                        : String.Empty,
                    RemoteEndpoint = result.RemoteEndPoint,
                    ElapsedMS = elapsedMS
                };
            }

            public byte[] Datagram { get; private set; }

            public string Value { get; private set; }

            public IPEndPoint RemoteEndpoint { get; private set; }

            public long ElapsedMS { get; private set; }

            public bool IsSuccess { get; private set; } = false;

            /// <summary>
            /// check Message when IsSucess == false
            /// </summary>
            public string ErrorMessage { get; private set; }
            public Exception Exception { get; private set; }
        }

        public Task<Response> SendAsync(string message)
        {
            return SendAsync(Encoding.UTF8.GetBytes(message));
        }

        public Task<Response> SendAsync(byte[] datagram)
        {
            return Task.Run(async () =>
            {
                var timer = new Stopwatch();
                try
                {
                    if (_client == null || !IsConnected)
                    {
                        return Response.FromFailure("not connected");
                    }

                    var spinWait = new SpinWait();
                    timer.Start();

                    await _client.SendAsync(datagram, datagram.Length);

                    while (_client.Available == 0 && timer.Elapsed <= _responseTimeout)
                    {
                        spinWait.SpinOnce();
                    }

                    timer.Stop();

                    if (_client.Available == 0)
                    {
                        throw new TimeoutException(_endPoint.ToString());
                    }

                    return Response.FromUdpReceiveResult(await _client.ReceiveAsync(), timer.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    return Response.FromException(ex, timer.ElapsedMilliseconds);
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
                    Disconnect();
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
