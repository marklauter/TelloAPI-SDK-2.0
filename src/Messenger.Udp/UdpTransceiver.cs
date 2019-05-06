using Sumo.Retry.Policies;
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

        public UdpTransceiver(RetryPolicy retryPolicy, IPAddress ipAddress, int port)
            : base(retryPolicy)
        {
            _endPoint = new IPEndPoint(ipAddress, port);
        }

        public void Dispose()
        {
            _client.Dispose();
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
                stopwatch.Stop();
            });

            if (_client.Available == 0)
            {
                throw new TimeoutException(stopwatch.Elapsed.ToString());
            }

            var receiveResult = await _client.ReceiveAsync();

            return new Response(request, receiveResult.Buffer, stopwatch.Elapsed);
        }
    }
}