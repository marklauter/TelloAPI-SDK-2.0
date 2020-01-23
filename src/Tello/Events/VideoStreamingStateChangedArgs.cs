// <copyright file="VideoStreamingStateChangedArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.Events
{
    public sealed class VideoStreamingStateChangedArgs : EventArgs
    {
        public VideoStreamingStateChangedArgs(bool isStreaming)
        {
            this.IsStreaming = isStreaming;
        }

        public bool IsStreaming { get; }
    }
}
