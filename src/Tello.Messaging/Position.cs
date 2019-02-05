using System;

namespace Tello.Messaging
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
            Z = position.Z;
            Heading = position.Heading;
        }

        public int X { get; set; } = 0;
        public int Y { get; set; } = 0;
        public int Z { get; set; } = 0;
        public int Heading { get; set; } = 0;

        public override string ToString()
        {
            return $"X:{X}, Y:{Y}, Z:{Z}, Hd:{Heading}";
        }
    }
}
