// <copyright file="TestHobbsMeter.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tello.State;

namespace Tello.Entities.Sqlite.Test
{
    public class TestHobbsMeter : TestObservation, IHobbsMeter
    {
        public int DistanceTraversedInCm { get; } = 1;

        public int MotorTimeInSeconds { get; } = 1;

        public double FlightTimeRemainingInMinutes => (15 * 60 - this.MotorTimeInSeconds) / 60.0;
    }
}
