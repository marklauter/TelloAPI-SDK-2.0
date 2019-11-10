// <copyright file="VideoTransmitter.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Linq;

namespace Tello.Simulator.Messaging
{
    internal sealed class VideoTransmitter : Transmitter
    {
        public void AddVideoSegment(byte[] segment)
        {
            if (segment != null)
            {
                this.available = 0;
                this.Buffer = this.Buffer.Union(segment).ToArray();
            }
        }
    }
}
