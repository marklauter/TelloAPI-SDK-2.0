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
    public sealed class Messenger : IMessenger
    {
        internal Messenger(string ip, int port) : base()
        {
            if (String.IsNullOrEmpty(ip))
            {
                throw new ArgumentNullException(nameof(ip));
            }

            _endPoint = new IPEndPoint(IPAddress.Parse(ip), port);
        }

        public MessengerStates State { get; private set; } = MessengerStates.Disconnected;
        public void ClearError() { State = MessengerStates.Disconnected; }

        private readonly IPEndPoint _endPoint;
        private UdpClient _client = null;

        public void Connect()
        {
            if (State == MessengerStates.Disconnected)
            {
                State = MessengerStates.Connecting;

                if (!IsNetworkAvailable)
                {
                    throw new NetworkUnavailableException("Device must be connected to Tello's WIFI.");
                }

                _client?.Dispose();
                _client = new UdpClient();
                _client.Connect(_endPoint);
                State = MessengerStates.Connected;
            }
        }

        public void Disconnect()
        {
            if (State != MessengerStates.Disconnected)
            {
                State = MessengerStates.Disconnected;
                if (_client != null)
                {
                    _client.Close();
                    _client.Dispose();
                    _client = null;
                }
            }
        }

        public bool IsNetworkAvailable => NetworkInterface.GetIsNetworkAvailable();

        public Task<IResponse> SendAsync(IRequest request)
        {
            return Task.Run(async () =>
            {
                if (State != MessengerStates.Connected)
                {
                    return Response.FromFailure(request, "Unable to send request.");
                }

                var spinWait = new SpinWait();
                var timer = Stopwatch.StartNew();
                try
                {
                    await _client.SendAsync(request.Data, request.Data.Length);
                    while (_client.Available == 0 && timer.Elapsed <= request.Timeout)
                    {
                        spinWait.SpinOnce();
                    }
                    timer.Stop();

                    if (_client.Available == 0)
                    {
                        throw new TimeoutException(_endPoint.ToString());
                    }

                    var response = await _client.ReceiveAsync();
                    return Response.FromUdpReceiveResult(request, response.Buffer, timer.Elapsed);
                }
                catch (Exception ex)
                {
                    return Response.FromException(request, ex, timer.Elapsed);
                }
            });
        }


    }
}
