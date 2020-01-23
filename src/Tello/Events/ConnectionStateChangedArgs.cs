// <copyright file="ConnectionStateChangedArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.Events
{
    public sealed class ConnectionStateChangedArgs : EventArgs
    {
        public ConnectionStateChangedArgs(bool isConnected)
        {
            this.IsConnected = isConnected;
        }

        public bool IsConnected { get; }
    }
}
