// <copyright file="TestAirSpeed.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tello.State;

namespace Tello.Entities.Sqlite.Test
{
    public class TestAirSpeed : TestObservation, IAirSpeed
    {
        public int SpeedX { get; } = 1;

        public int SpeedY { get; } = 1;

        public int SpeedZ { get; } = 1;

        public double AccelerationX { get; } = 1;

        public double AccelerationY { get; } = 1;

        public double AccelerationZ { get; } = 1;
    }
}
