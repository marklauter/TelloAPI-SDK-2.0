// <copyright file="AirSpeed.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.State
{
    internal sealed class AirSpeed : IAirSpeed
    {
        public AirSpeed(IAirSpeed airspeed)
        {
            if (airspeed == null)
            {
                throw new ArgumentNullException(nameof(airspeed));
            }

            this.AccelerationX = airspeed.AccelerationX;
            this.AccelerationY = airspeed.AccelerationY;
            this.AccelerationZ = airspeed.AccelerationZ;
            this.SpeedX = airspeed.SpeedX;
            this.SpeedY = airspeed.SpeedY;
            this.SpeedZ = airspeed.SpeedZ;
            this.Timestamp = airspeed.Timestamp;
        }

        public AirSpeed(ITelloState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            this.AccelerationX = state.AccelerationX;
            this.AccelerationY = state.AccelerationY;
            this.AccelerationZ = state.AccelerationZ;
            this.SpeedX = state.SpeedX;
            this.SpeedY = state.SpeedY;
            this.SpeedZ = state.SpeedZ;
            this.Timestamp = state.Timestamp;
        }

        public int SpeedX { get; }

        public int SpeedY { get; }

        public int SpeedZ { get; }

        public double AccelerationX { get; }

        public double AccelerationY { get; }

        public double AccelerationZ { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"X: {this.SpeedX} cm/s, Y: {this.SpeedY} cm/s, Z: {this.SpeedZ} cm/s, AX: {this.AccelerationX} cm/s/s, , AY: {this.AccelerationY} cm/s/s, , AZ: {this.AccelerationZ} cm/s/s";
        }
    }
}
