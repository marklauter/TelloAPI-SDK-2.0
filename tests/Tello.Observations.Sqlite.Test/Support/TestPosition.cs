// <copyright file="TestPosition.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tello.State;

namespace Tello.Entities.Sqlite.Test
{
    public class TestPosition : TestObservation, IPosition
    {
        public int AltitudeAGLInCm { get; } = 1;

        public double BarometricPressueInCm { get; } = 1;

        public int Heading { get; } = 1;

        public double X { get; } = 1;

        public double Y { get; } = 1;
    }
}
