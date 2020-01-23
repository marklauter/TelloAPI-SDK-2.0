// <copyright file="ReceiveResult.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Messenger.Simulator;

namespace Tello.Simulator.Messaging
{
    internal class ReceiveResult : IReceiveResult
    {
        public ReceiveResult(byte[] buffer)
        {
            this.Buffer = buffer ?? throw new System.ArgumentNullException(nameof(buffer));
        }

        public byte[] Buffer { get; }
    }
}