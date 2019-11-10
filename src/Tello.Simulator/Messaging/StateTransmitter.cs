// <copyright file="StateTransmitter.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Text;
using Tello.State;

namespace Tello.Simulator.Messaging
{
    internal sealed class StateTransmitter : Transmitter
    {
        public void SetState(ITelloState telloState)
        {
            if (telloState != null)
            {
                this.available = 0;
                this.Buffer = Encoding.UTF8.GetBytes(telloState.ToString());
            }
        }
    }
}
