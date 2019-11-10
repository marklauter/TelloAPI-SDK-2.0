// <copyright file="Envelope.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public class Envelope : IEnvelope
    {
        public Envelope(byte[] data)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            this.Data = data;

            this.Timestamp = DateTime.UtcNow;
            this.Id = Guid.NewGuid();
        }

        public Guid Id { get; }

        public DateTime Timestamp { get; }

        public byte[] Data { get; }
    }
}
