using System;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    public class Position
    {
        public Position() { }

        public Position(Position position)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            X = position.X;
            Y = position.Y;
            Height = position.Height;
            Heading = position.Heading;
        }

        public double X { get; private set; } = 0;
        public double Y { get; private set; } = 0;
        public int Height { get; internal set; } = 0;
        public int Heading { get; private set; } = 0;

        public override string ToString()
        {
            return $"X:{X.ToString("F2")}, Y:{Y.ToString("F2")}, Z:{Height}, Hd:{Heading}";
        }

        internal void Go(int x, int y)
        {
            X += x;
            Y += y;
        }

        internal void Move(Commands direction, int distanceInCm)
        {
            //Debug.WriteLine($"move {direction}, distance {distanceInCm}, heading {Heading}");

            var heading = Heading;
            switch (direction)
            {
                case Commands.Right:
                    heading += 90;
                    break;
                case Commands.Back:
                    heading += 180;
                    break;
                case Commands.Left:
                    heading += 270;
                    break;
            }
            var radians = Heading * Math.PI / 180;

            var yComponent = Math.Cos(radians) * distanceInCm;
            var xComponent = Math.Sin(radians) * distanceInCm;
            //Debug.WriteLine($"components (X,Y): ({xComponent.ToString("F2")},{yComponent.ToString("F2")})");

            Y += yComponent;
            X += xComponent;
            //Debug.WriteLine($"New (X,Y): ({X.ToString("F2")},{Y.ToString("F2")})");
        }

        internal void Turn(Commands direction, int degrees)
        {
            if (degrees < 1 || degrees > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            //Debug.WriteLine($"Old Heading: {Heading}");
            switch (direction)
            {
                case Commands.ClockwiseTurn:
                    Heading += degrees;
                    if (Heading >= 360)
                    {
                        Heading -= 360;
                    }
                    break;
                case Commands.CounterClockwiseTurn:
                    Heading -= degrees;
                    if (Heading < 0)
                    {
                        Heading += 360;
                    }
                    break;
            }
            //Debug.WriteLine($"New Heading: {Heading}");
        }
    }
}
