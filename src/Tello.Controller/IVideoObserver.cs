// <copyright file="IVideoObserver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Messenger;

// using System.Collections.Generic;
using Tello.Events;

namespace Tello.Controller
{
    public interface IVideoObserver : IObserver<IEnvelope>
    {
        event EventHandler<VideoSampleReadyArgs> VideoSampleReady;

        event EventHandler<Exception> ExceptionThrown;

        // Queue<IEnvelope> VideoSegments();
    }
}
