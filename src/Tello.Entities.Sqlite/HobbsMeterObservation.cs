// <copyright file="HobbsMeterObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    public sealed class HobbsMeterObservation : Observation, IHobbsMeter
    {
        public HobbsMeterObservation()
            : base()
        {
        }

        public HobbsMeterObservation(
            IObservationGroup group,
            ITelloState state)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).HobbsMeter)
        {
        }

        public HobbsMeterObservation(
            int groupId,
            ITelloState state)
            : this(
                  groupId,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).HobbsMeter)
        {
        }

        private HobbsMeterObservation(
            int groupId,
            DateTime timestamp,
            IHobbsMeter hobbsMeter)
            : base(
                  groupId,
                  timestamp)
        {
            if (hobbsMeter == null)
            {
                throw new ArgumentNullException(nameof(hobbsMeter));
            }

            this.DistanceTraversedInCm = hobbsMeter.DistanceTraversedInCm;
            this.MotorTimeInSeconds = hobbsMeter.MotorTimeInSeconds;
        }

        public int DistanceTraversedInCm { get; set; }

        public int MotorTimeInSeconds { get; set; }

        [SQLite.Ignore]
        public double FlightTimeRemainingInMinutes { get; }
    }
}
