using System;

namespace Tello.Directions
{
    public class CardinalDirection
    {
        public CardinalDirection() : this(CardinalDirections.Front) { }

        public CardinalDirection(CardinalDirections direction)
        {
            Value = direction;
        }

        public CardinalDirections Value { get; }

        #region operators
        public static implicit operator CardinalDirection(CardinalDirections direction)
        {
            return new CardinalDirection(direction);
        }

        public static implicit operator CardinalDirections(CardinalDirection direction)
        {
            return direction.Value;
        }

        public static explicit operator char(CardinalDirection direction)
        {
            switch (direction.Value)
            {
                case CardinalDirections.Left:
                    return 'l';
                case CardinalDirections.Right:
                    return 'r';
                case CardinalDirections.Front:
                    return 'f';
                case CardinalDirections.Back:
                    return 'b';
                default:
                    throw new NotSupportedException(direction.ToString());
            }
        }
        #endregion

        public override string ToString()
        {
            switch (Value)
            {
                case CardinalDirections.Left:
                    return "l";
                case CardinalDirections.Right:
                    return "r";
                case CardinalDirections.Front:
                    return "f";
            }
            return "b";
        }
    }
}
