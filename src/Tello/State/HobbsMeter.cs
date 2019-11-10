// <copyright file="HobbsMeter.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.State
{
    internal sealed class HobbsMeter : IHobbsMeter
    {
        public HobbsMeter(IHobbsMeter hobbsMeter)
        {
            if (hobbsMeter == null)
            {
                throw new ArgumentNullException(nameof(hobbsMeter));
            }

            this.DistanceTraversedInCm = hobbsMeter.DistanceTraversedInCm;
            this.MotorTimeInSeconds = hobbsMeter.MotorTimeInSeconds;
            this.Timestamp = hobbsMeter.Timestamp;
        }

        public HobbsMeter(ITelloState state)
        {
            this.DistanceTraversedInCm = state.DistanceTraversedInCm;
            this.MotorTimeInSeconds = state.MotorTimeInSeconds;
            this.Timestamp = state.Timestamp;
        }

        public int DistanceTraversedInCm { get; }

        public int MotorTimeInSeconds { get; }

        // documentation says there's ~ 15 minutes of battery
        public double FlightTimeRemainingInMinutes => (15 * 60 - this.MotorTimeInSeconds) / 60.0;

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"MT: {this.MotorTimeInSeconds} s, D: {this.DistanceTraversedInCm} cm";
        }
    }
}
