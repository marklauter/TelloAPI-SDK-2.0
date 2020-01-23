// <copyright file="AttitudeObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    public sealed class AttitudeObservation : Observation, IAttitude
    {
        public AttitudeObservation()
            : base()
        {
        }

        public AttitudeObservation(
            IObservationGroup group,
            ITelloState state)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).Attitude)
        {
        }

        public AttitudeObservation(
            int groupId,
            ITelloState state)
            : this(
                  groupId,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).Attitude)
        {
        }

        private AttitudeObservation(
            int groupId,
            DateTime timestamp,
            IAttitude attitude)
            : base(
                  groupId,
                  timestamp)
        {
            if (attitude == null)
            {
                throw new ArgumentNullException(nameof(attitude));
            }

            this.Pitch = attitude.Pitch;
            this.Roll = attitude.Roll;
            this.Yaw = attitude.Yaw;
        }

        public int Pitch { get; set; }

        public int Roll { get; set; }

        public int Yaw { get; set; }
    }
}
