// <copyright file="UdpTransceiver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Udp
{
    public sealed class UdpTransceiver : Transceiver, ITransceiver
    {
        private readonly IPEndPoint endPoint;
        private readonly UdpClient client = new UdpClient();

        public UdpTransceiver(IPAddress ipAddress, int port)
            : base()
        {
            this.endPoint = new IPEndPoint(ipAddress, port);
        }

        protected override async Task<IResponse> Send(IRequest request)
        {
            await this.client.SendAsync(request.Data, request.Data.Length, this.endPoint);

            if (request.NoWait)
            {
                return new Response(request, TimeSpan.Zero, true);
            }

            var stopwatch = Stopwatch.StartNew();
            await Task.Run(() =>
            {
                var spinWait = default(SpinWait);
                while (this.client.Available == 0 && stopwatch.Elapsed <= request.Timeout)
                {
                    spinWait.SpinOnce();
                }
            });
            stopwatch.Stop();

            if (this.client.Available == 0)
            {
                throw new TimeoutException(stopwatch.Elapsed.ToString());
            }

            return new Response(
                request,
                (await this.client.ReceiveAsync()).Buffer,
                stopwatch.Elapsed);
        }

        public override void Dispose()
        {
            this.client.Dispose();
        }
    }
}