// <copyright file="TestAttitude.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tello.State;

namespace Tello.Entities.Sqlite.Test
{
    public class TestAttitude : TestObservation, IAttitude
    {
        public int Pitch { get; } = 1;

        public int Roll { get; } = 1;

        public int Yaw { get; } = 1;
    }
}
