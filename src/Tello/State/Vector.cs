// <copyright file="Vector.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.State
{
    public class Vector
    {
        public Vector(int heading, double x, double y)
        {
            this.Heading = heading;
            this.X = x;
            this.Y = y;
        }

        public Vector()
            : this(0, 0, 0)
        {
        }

        public int Heading { get; }

        public double X { get; }

        public double Y { get; }

        public Vector Go(int xDelta, int yDelta)
        {
            return new Vector(this.Heading, this.X + xDelta, this.Y + yDelta);
        }

        public Vector Move(CardinalDirections direction, int distance)
        {
            var heading = this.Heading;
            switch (direction)
            {
                case CardinalDirections.Right:
                    heading += 90;
                    break;
                case CardinalDirections.Back:
                    heading += 180;
                    break;
                case CardinalDirections.Left:
                    heading += 270;
                    break;
            }

            var radians = heading * Math.PI / 180;
            var adjacent = Math.Cos(radians) * distance;
            var opposite = Math.Sin(radians) * distance;

            return new Vector(this.Heading, Math.Round(this.X + adjacent, 2), Math.Round(this.Y + opposite, 2));
        }

        public Vector Turn(ClockDirections direction, int degrees)
        {
            if (degrees <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            degrees %= 360;

            var heading = this.Heading;
            switch (direction)
            {
                case ClockDirections.Clockwise:
                    heading += degrees;
                    if (heading >= 360)
                    {
                        heading -= 360;
                    }

                    break;
                case ClockDirections.CounterClockwise:
                    heading -= degrees;
                    if (heading < 360)
                    {
                        heading += 360;
                    }

                    break;
            }

            return new Vector(heading, this.X, this.Y);
        }

        public override string ToString()
        {
            return $"P({this.X}, {this.Y}) \u0398({this.Heading})";
        }
    }
}
