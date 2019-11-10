// <copyright file="PositionObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    public sealed class PositionObservation : Observation, IPosition
    {
        public PositionObservation()
            : base()
        {
        }

        public PositionObservation(
            IObservationGroup group,
            ITelloState state)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  state)
        {
        }

        public PositionObservation(
            int groupId,
            ITelloState state)
            : this(
                  groupId,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).Position)
        {
        }

        private PositionObservation(
            int groupId,
            DateTime timestamp,
            IPosition position)
            : base(
                  groupId,
                  timestamp)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            this.AltitudeAGLInCm = position.AltitudeAGLInCm;
            this.BarometricPressueInCm = position.BarometricPressueInCm;
            this.Heading = position.Heading;
            this.X = position.X;
            this.Y = position.Y;
        }

        public int AltitudeAGLInCm { get; set; }

        public double BarometricPressueInCm { get; set; }

        public int Heading { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
