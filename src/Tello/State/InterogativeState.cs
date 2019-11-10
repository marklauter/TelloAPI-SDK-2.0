// <copyright file="InterogativeState.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tello.State
{
    public sealed class InterogativeState
    {
        public int Speed { get; set; }

        public int Battery { get; set; }

        public int Time { get; set; }

        public string WIFISnr { get; set; }

        public string SdkVersion { get; set; }

        public string SerialNumber { get; set; }
    }
}
