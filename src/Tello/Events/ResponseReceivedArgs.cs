// <copyright file="ResponseReceivedArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.Messaging;

namespace Tello.Events
{
    public sealed class ResponseReceivedArgs : EventArgs
    {
        public ResponseReceivedArgs(TelloResponse response)
        {
            this.Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public TelloResponse Response { get; }
    }
}
