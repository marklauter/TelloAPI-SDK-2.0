﻿// <copyright file="Position.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.State
{
    internal sealed class Position : IPosition
    {
        #region static
        private static double altitudeDelta;

        public static void CalibrateAltimeter(double barometricAltitudeInCm, double actualAltitudeInCm)
        {
            altitudeDelta = actualAltitudeInCm - barometricAltitudeInCm;
        }
        #endregion

        private Position(int? altitudeAGLInCm, double? altitudeMSLInCm, int? heading, double? x, double? y, DateTime? timestamp)
        {
            if (altitudeAGLInCm.HasValue)
            {
                this.AltitudeAGLInCm = altitudeAGLInCm.Value;
            }

            if (altitudeMSLInCm.HasValue)
            {
                this.BarometricPressueInCm = altitudeMSLInCm.Value;
            }

            if (heading.HasValue)
            {
                this.Heading = heading.Value;
            }

            if (x.HasValue)
            {
                this.X = x.Value;
            }

            if (y.HasValue)
            {
                this.Y = y.Value;
            }

            if (timestamp.HasValue)
            {
                this.Timestamp = timestamp.Value;
            }
        }

        public Position(IPosition position)
            : this(
                position?.AltitudeAGLInCm,
                position?.BarometricPressueInCm,
                position?.Heading,
                position?.X,
                position?.Y,
                position?.Timestamp)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }
        }

        // todo: fix this() calls so they look like : (state ?? throw new ArgumentNullException(nameof(state))).HeightInCm
        public Position(ITelloState state, Vector vector)
            : this(
                 state?.HeightInCm,
                 state?.BarometerInCm + altitudeDelta,
                 vector?.Heading,
                 vector?.X,
                 vector?.Y,
                 state?.Timestamp)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
        }

        public Position(ITelloState state)
            : this(
                 state?.HeightInCm,
                 state?.BarometerInCm + altitudeDelta,
                 0,
                 0,
                 0,
                 state?.Timestamp)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }
        }

        /// <summary>
        /// altitude - AGL stands for "above ground level" - values would typically be generated by a radar altimeter
        /// value is measured in cm.
        /// </summary>
        public int AltitudeAGLInCm { get; }

        /// <summary>
        /// altitude - MSL stands for "mean sea level" - values would typically be generated by a barometric pressure based altimeter
        /// for this value to be meaningful you have to zero the altimeter by calling the static method ZeroAltimeter
        /// value is measured in cm.
        /// </summary>
        public double BarometricPressueInCm { get; }

        /// <summary>
        /// heading in degrees, from starting position, not from north.
        /// </summary>
        public int Heading { get; }

        /// <summary>
        /// estimated X value relative to starting position based on movement commands, unless the Mission Pad is enabled, and then it's the X value based on drone's position relative to the Mission Pad.
        /// </summary>
        public double X { get; }

        /// <summary>
        /// estimated Y value relative to starting position based on movement commands, unless the Mission Pad is enabled, and then it's the Y value based on drone's position relative to the Mission Pad.
        /// </summary>
        public double Y { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            var mslFt = this.BarometricPressueInCm / 30.48;
            return $"X: {this.X.ToString("F2")} cm, Y: {this.Y.ToString("F2")} cm, MSL: {mslFt:F2} ft, AGL: {this.AltitudeAGLInCm} cm, Hd: {this.Heading} deg, AD: {altitudeDelta:F2} cm";
        }
    }
}
