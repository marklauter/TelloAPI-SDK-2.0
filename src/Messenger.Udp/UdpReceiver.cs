// <copyright file="UdpReceiver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Udp
{
    public sealed class UdpReceiver : Receiver
    {
        private readonly int port;

        public UdpReceiver(int port)
        {
            this.port = port;
        }

        protected override async Task Listen(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            var wait = default(SpinWait);

            using (var client = new UdpClient(this.port))
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    try
                    {
                        if (client.Available > 0)
                        {
                            this.MessageReceived(new Envelope((await client.ReceiveAsync()).Buffer));
                        }
                        else
                        {
                            wait.SpinOnce();
                        }
                    }
                    catch (Exception ex)
                    {
                        this.ExceptionThrown(ex);
                    }
                }
            }
        }
    }
}
