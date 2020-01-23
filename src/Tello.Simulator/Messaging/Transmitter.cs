// <copyright file="Transmitter.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using Messenger.Simulator;

namespace Tello.Simulator.Messaging
{
    internal class Transmitter : IDroneTransmitter
    {
        private byte[] buffer = Array.Empty<byte>();

        protected byte[] Buffer
        {
            get => this.buffer;
            set
            {
                this.buffer = value ?? throw new System.ArgumentNullException(nameof(value));
                this.available = this.buffer.Length;
            }
        }

        protected int available = 0;

        public int Available => this.available;

        public Task<IReceiveResult> ReceiveAsync()
        {
            this.available = 0;
            var result = new ReceiveResult(this.Buffer) as IReceiveResult;
            this.Buffer = Array.Empty<byte>();
            return Task.FromResult(result);
        }
    }
}
