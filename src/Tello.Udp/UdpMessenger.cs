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
    public class UdpMessenger : IMessengerService
    {
        public UdpMessenger() : base()
        {
            _telloEndPoint = new IPEndPoint(IPAddress.Parse("192.168.10.1"), 8889);

            StateServer = new StateServer(8890);
            var videoServer = new VideoServer(30, 10, 11111);
            VideoServer = videoServer;
            VideoSampleProvider = videoServer;
        }

        public IMessageRelayService<IRawDroneState> StateServer { get; }
        public IMessageRelayService<IVideoSample> VideoServer { get; }
        public IVideoSampleProvider VideoSampleProvider { get; }

        public MessengerStates MessengerState { get; private set; } = MessengerStates.Disconnected;

        private readonly IPEndPoint _telloEndPoint;
        private UdpClient _client = null;

        public void Connect()
        {
            if (MessengerState == MessengerStates.Disconnected)
            {
                MessengerState = MessengerStates.Connecting;

                if (!IsNetworkAvailable)
                {
                    throw new TelloUnavailableException("Device must be connected to Tello's WIFI.");
                }

                _client?.Dispose();
                _client = new UdpClient();
                _client.Connect(_telloEndPoint);
                MessengerState = MessengerStates.Connected;
            }
        }

        public void Disconnect()
        {
            if (MessengerState != MessengerStates.Disconnected)
            {
                MessengerState = MessengerStates.Disconnected;
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
                var stopwatch = Stopwatch.StartNew();
                try
                {
                    if (MessengerState != MessengerStates.Connected)
                    {
                        throw new InvalidOperationException("Not connected.");
                    }

                    await _client.SendAsync(request.Data, request.Data.Length);

                    var spinWait = new SpinWait();
                    while (_client.Available == 0 && stopwatch.Elapsed <= request.Timeout)
                    {
                        spinWait.SpinOnce();
                    }
                    stopwatch.Stop();

                    if (_client.Available == 0)
                    {
                        throw new TimeoutException(stopwatch.Elapsed.ToString());
                    }

                    var response = await _client.ReceiveAsync();
                    return Response.FromData(request, response.Buffer, stopwatch.Elapsed);
                }
                catch (Exception ex)
                {
                    return Response.FromException(request, ex, stopwatch.Elapsed);
                }
            });
        }
    }
}
