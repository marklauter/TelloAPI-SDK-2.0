// <copyright file="TelloResponse.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using Messenger;

namespace Tello.Messaging
{
    public sealed class TelloResponse : Response<string>
    {
        public TelloResponse(IResponse response)
            : base(response)
        {
        }

        public TelloResponse(
            IRequest request,
            Exception exception,
            TimeSpan timeTaken)
            : base(
                  request,
                  exception,
                  timeTaken)
        {
        }

        public TelloResponse(
            IRequest request,
            byte[] data,
            TimeSpan timeTaken)
            : base(
                  request,
                  data,
                  timeTaken)
        {
        }

        protected override string Deserialize(byte[] data)
        {
            return data != null && data.Length > 0
                ? Encoding.UTF8.GetString(data).Trim()
                : String.Empty;
        }
    }
}
