// <copyright file="Response{T}.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public abstract class Response<T> : Response, IResponse<T>
    {
        public Response(IResponse response)
            : base(response)
        {
            this.Message = this.Deserialize(this.Data);
        }

        public Response(IRequest request, Exception exception, TimeSpan timeTaken)
            : base(request, exception, timeTaken)
        {
            this.Message = this.Deserialize(this.Data);
        }

        public Response(IRequest request, byte[] data, TimeSpan timeTaken)
            : base(request, data, timeTaken)
        {
            this.Message = this.Deserialize(this.Data);
        }

        protected abstract T Deserialize(byte[] data);

        public T Message { get; }
    }
}
