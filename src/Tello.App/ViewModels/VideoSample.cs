// <copyright file="VideoSample.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.Threading;
using Tello.App.MvvM;
using Tello.Controller;

namespace Tello.App.ViewModels
{
    public sealed class VideoSample
    {
        public byte[] Buffer { get; }

        public TimeSpan TimeIndex { get; }

        public TimeSpan Duration { get; }

        public int Length => this.Buffer.Length;

        public VideoSample()
            : this(
                 Array.Empty<byte>(),
                 TimeSpan.FromSeconds(0),
                 TimeSpan.FromSeconds(0))
        {
        }

        public VideoSample(byte[] buffer, TimeSpan timeIndex, TimeSpan duration)
        {
            this.Buffer = buffer;
            this.TimeIndex = timeIndex;
            this.Duration = duration;
        }
    }
}
