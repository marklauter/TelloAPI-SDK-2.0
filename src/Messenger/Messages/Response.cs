// <copyright file="Response.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Messenger
{
    public class Response : IResponse
    {
        protected Response(IResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            this.Id = response.Id;
            this.Timestamp = response.Timestamp;
            this.Data = response.Data;

            this.Request = response.Request;
            this.Exception = response.Exception;
            this.StatusMessage = response.StatusMessage;
            this.TimeTaken = response.TimeTaken;
            this.Success = response.Success;
        }

        public Response(IRequest request, TimeSpan timeTaken, bool success)
        {
            this.Request = request ?? throw new ArgumentNullException(nameof(request));
            this.TimeTaken = timeTaken;
            this.Success = success;
            this.Timestamp = DateTime.UtcNow;
            this.Id = Guid.NewGuid();
        }

        public Response(IRequest request, Exception exception, TimeSpan timeTaken)
            : this(request, timeTaken, false)
        {
            this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            this.StatusMessage = $"{exception.GetType().Name} - {exception.Message}";
        }

        public Response(IRequest request, byte[] data, TimeSpan timeTaken)
            : this(request, timeTaken, true)
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

        public IRequest Request { get; }

        public TimeSpan TimeTaken { get; }

        public bool Success { get; }

        public string StatusMessage { get; }

        public Exception Exception { get; }

        public byte[] Data { get; }

        public DateTime Timestamp { get; }

        public Guid Id { get; }
    }
}
