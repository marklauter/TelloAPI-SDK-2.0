// <copyright file="IStateObserver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Messenger;
using Tello.Events;
using Tello.State;

namespace Tello.Controller
{
    public interface IStateObserver : IObserver<IEnvelope>
    {
        event EventHandler<StateChangedArgs> StateChanged;

        event EventHandler<Exception> ExceptionThrown;

        void UpdatePosition(object sender, PositionChangedArgs e);

        ITelloState State { get; }
    }
}
