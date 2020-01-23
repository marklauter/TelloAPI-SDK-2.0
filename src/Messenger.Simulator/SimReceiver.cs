// <copyright file="SimReceiver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public sealed class SimReceiver : Receiver
    {
        private readonly IDroneTransmitter transmitter;

        public SimReceiver(IDroneTransmitter transmitter)
        {
            this.transmitter = transmitter ?? throw new ArgumentNullException(nameof(transmitter));
        }

        protected override async Task Listen(CancellationToken cancellationToken)
        {
            if (cancellationToken == null)
            {
                throw new ArgumentNullException(nameof(cancellationToken));
            }

            var wait = default(SpinWait);

            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    if (this.transmitter.Available > 0)
                    {
                        this.MessageReceived(new Envelope((await this.transmitter.ReceiveAsync()).Buffer));
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
