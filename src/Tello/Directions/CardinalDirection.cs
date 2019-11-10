// <copyright file="CardinalDirection.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello
{
    public class CardinalDirection
    {
        public CardinalDirection()
            : this(CardinalDirections.Front)
        {
        }

        public CardinalDirection(CardinalDirections direction)
        {
            this.Value = direction;
        }

        public CardinalDirections Value { get; }

        #region operators
        public static explicit operator CardinalDirection(CardinalDirections direction)
        {
            return new CardinalDirection(direction);
        }

        public static explicit operator CardinalDirections(CardinalDirection direction)
        {
            return direction.Value;
        }

        public static implicit operator char(CardinalDirection direction)
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

        public static implicit operator CardinalDirection(char direction)
        {
            switch (Char.ToLowerInvariant(direction))
            {
                case 'l':
                    return new CardinalDirection(CardinalDirections.Left);
                case 'r':
                    return new CardinalDirection(CardinalDirections.Right);
                case 'f':
                    return new CardinalDirection(CardinalDirections.Front);
                case 'b':
                    return new CardinalDirection(CardinalDirections.Back);
                default:
                    throw new NotSupportedException(direction.ToString());
            }
        }
        #endregion

        public override string ToString()
        {
            switch (this.Value)
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
