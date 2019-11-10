// <copyright file="AirSpeedObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    public sealed class AirSpeedObservation : Observation, IAirSpeed
    {
        public AirSpeedObservation()
            : base()
        {
        }

        public AirSpeedObservation(
            IObservationGroup group,
            ITelloState state)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  state ?? throw new ArgumentNullException(nameof(state)))
        {
        }

        public AirSpeedObservation(
            int groupId,
            ITelloState state)
            : this(
                  groupId,
                  state.Timestamp,
                  state.AirSpeed)
        {
        }

        private AirSpeedObservation(
            int groupId,
            DateTime timestamp,
            IAirSpeed airspeed)
            : base(
                  groupId,
                  timestamp)
        {
            if (airspeed == null)
            {
                throw new ArgumentNullException(nameof(airspeed));
            }

            this.SpeedX = airspeed.SpeedX;
            this.SpeedY = airspeed.SpeedY;
            this.SpeedZ = airspeed.SpeedZ;
            this.AccelerationX = airspeed.AccelerationX;
            this.AccelerationY = airspeed.AccelerationY;
            this.AccelerationZ = airspeed.AccelerationZ;
        }

        public int SpeedX { get; set; }

        public int SpeedY { get; set; }

        public int SpeedZ { get; set; }

        public double AccelerationX { get; set; }

        public double AccelerationY { get; set; }

        public double AccelerationZ { get; set; }
    }
}
