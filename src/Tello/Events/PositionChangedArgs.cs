// <copyright file="PositionChangedArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Events
{
    public sealed class PositionChangedArgs : EventArgs
    {
        public PositionChangedArgs(Vector position)
        {
            this.Position = position;
        }

        public Vector Position { get; }
    }
}
