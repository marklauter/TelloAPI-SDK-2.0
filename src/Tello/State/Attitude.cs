// <copyright file="Attitude.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.State
{
    internal sealed class Attitude : IAttitude
    {
        public Attitude(IAttitude attitude)
        {
            if (attitude == null)
            {
                throw new ArgumentNullException(nameof(attitude));
            }

            this.Pitch = attitude.Pitch;
            this.Roll = attitude.Roll;
            this.Yaw = attitude.Yaw;
            this.Timestamp = attitude.Timestamp;
        }

        public Attitude(ITelloState state, bool useMissionPad = false)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (!useMissionPad)
            {
                this.Pitch = state.Pitch;
                this.Roll = state.Roll;
                this.Yaw = state.Yaw;
            }
            else
            {
                if (!state.MissionPadDetected)
                {
                    throw new ArgumentException($"{nameof(state)}.{nameof(ITelloState.MissionPadDetected)} == false");
                }

                this.Pitch = state.MissionPadPitch;
                this.Roll = state.MissionPadRoll;
                this.Yaw = state.MissionPadYaw;
            }

            this.Timestamp = state.Timestamp;
        }

        public int Pitch { get; }

        public int Roll { get; }

        public int Yaw { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"P: {this.Pitch} deg, R: {this.Roll} deg, Y: {this.Yaw} deg";
        }
    }
}
