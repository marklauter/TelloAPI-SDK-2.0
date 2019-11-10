// <copyright file="VideoSampleReadyArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Messenger;

namespace Tello.Events
{
    public sealed class VideoSampleReadyArgs : EventArgs
    {
        public VideoSampleReadyArgs(IEnvelope message)
        {
            this.Message = message ?? throw new ArgumentNullException(nameof(message));
        }

        public IEnvelope Message { get; }
    }
}
