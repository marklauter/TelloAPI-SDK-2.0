using System;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Udp
{
    public sealed class UdpMessenger : IMessenger
    {
        public UdpMessenger(string ip, int port, TimeSpan responseTimeout) : base()
        {
            if (String.IsNullOrEmpty(ip))
            {
                throw new ArgumentNullException(nameof(ip));
            }

            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);

            _responseTimeout = responseTimeout;
        }

        public MessengerStates State { get; private set; } = MessengerStates.Disconnected;
        public void ClearError() { State = MessengerStates.Disconnected; }

        private readonly IPEndPoint _endPoint;
        private readonly TimeSpan _responseTimeout;
        private UdpClient _client = null;

        public void Connect(TimeSpan timeout, Action<IMessenger> continuation = null)
        {
            if (State == MessengerStates.Disconnected)
            {
                State = MessengerStates.Connecting;
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
                    State = MessengerStates.Connected;

                    continuation?.Invoke(this);
                }
                catch (Exception ex)
                {
                    State = MessengerStates.Error;
                    Debug.WriteLine(ex);
                    throw;
                }
            }
        }

        public void Disconnect(Action<IMessenger> continuation)
        {
            if (State != MessengerStates.Disconnected)
            {
                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                    _client = null;
                }
                State = MessengerStates.Disconnected;
                continuation?.Invoke(this);
            }
        }

        public bool IsNetworkAvailable => NetworkInterface.GetIsNetworkAvailable();

        public Task<IResponse> SendAsync(IRequest request)
        {
            if (State != MessengerStates.Connected)
            {
                return Response.FromFailure("Unable to send request.")
            }

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


    }
}
