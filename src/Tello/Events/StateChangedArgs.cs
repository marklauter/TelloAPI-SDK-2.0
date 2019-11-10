// <copyright file="StateChangedArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Events
{
    public sealed class StateChangedArgs : EventArgs
    {
        public StateChangedArgs(ITelloState state)
        {
            this.State = state ?? throw new ArgumentNullException(nameof(state));
        }

        public ITelloState State { get; }
    }
}
