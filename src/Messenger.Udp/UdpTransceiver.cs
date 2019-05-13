using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Udp
{
    public sealed class UdpTransceiver : Transceiver, ITransceiver, IDisposable
    {
        private readonly IPEndPoint _endPoint;
        private readonly UdpClient _client = new UdpClient();

        public UdpTransceiver(IPAddress ipAddress, int port)
            : base()
        {
            _endPoint = new IPEndPoint(ipAddress, port);
        }

        protected override async Task<IResponse> Send(IRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            await _client.SendAsync(request.Data, request.Data.Length, _endPoint);

            await Task.Run(() =>
            {
                var spinWait = new SpinWait();
                while (_client.Available == 0 && stopwatch.Elapsed <= request.Timeout)
                {
                    spinWait.SpinOnce();
                }
            });

            stopwatch.Stop();

            if (_client.Available == 0)
            {
                throw new TimeoutException(stopwatch.Elapsed.ToString());
            }

            return new Response(
                request,
                (await _client.ReceiveAsync()).Buffer,
                stopwatch.Elapsed);
        }

        public void Dispose()
        {
            _client.Dispose();
        }

    }
}