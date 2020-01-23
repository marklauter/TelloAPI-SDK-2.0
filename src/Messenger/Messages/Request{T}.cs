// <copyright file="Request{T}.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public abstract class Request<T> : Request, IRequest<T>
    {
        public Request(T message, TimeSpan? timeout = null, bool noWait = false)
            : base(timeout, noWait)
        {
            this.Message = message;
            this.Data = this.Serialize(message);
        }

        protected abstract byte[] Serialize(T message);

        public T Message { get; }
    }
}
