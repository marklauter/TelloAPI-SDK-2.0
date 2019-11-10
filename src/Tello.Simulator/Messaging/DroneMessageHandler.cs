// <copyright file="DroneMessageHandler.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using System.Threading.Tasks;
using Messenger.Simulator;

namespace Tello.Simulator.Messaging
{
    internal sealed class DroneMessageHandler : IDroneMessageHandler
    {
        public DroneMessageHandler(Func<Command, string> commandReceived)
        {
            this.commandReceived = commandReceived ?? throw new ArgumentNullException(nameof(commandReceived));
        }

        private readonly Func<Command, string> commandReceived;

        public Task<byte[]> Invoke(byte[] buffer)
        {
            if (buffer != null)
            {
                var result = this.commandReceived((Command)buffer);
                return Task.FromResult(Encoding.UTF8.GetBytes(result));
            }
            else
            {
                return Task.FromResult(Encoding.UTF8.GetBytes("error"));
            }
        }
    }
}
