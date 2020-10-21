// <copyright file="TelloRequest.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using Messenger;

namespace Tello.Messaging
{
    public sealed class TelloRequest : Request<string>
    {
        public TelloRequest(Command command)
            : base(
                  (string)command,
                  (TimeSpan)command,
                  command.Rule.ExpectedResponse == Responses.None)
        {
        }

        protected override byte[] Serialize(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
