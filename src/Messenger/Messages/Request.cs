// <copyright file="Request.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public abstract class Request : IRequest
    {
        protected Request(TimeSpan? timeout = null, bool noWait = false)
        {
            this.Timeout = timeout ?? TimeSpan.MaxValue;
            this.Id = Guid.NewGuid();
            this.Timestamp = DateTime.UtcNow;
            this.NoWait = noWait;
        }

        public Request(byte[] data, TimeSpan? timeout = null, bool noWait = false)
            : this(timeout, noWait)
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
        }

        public byte[] Data { get; protected set; }

        public Guid Id { get; }

        public TimeSpan Timeout { get; }

        public DateTime Timestamp { get; }

        public bool NoWait { get; }
    }
}
