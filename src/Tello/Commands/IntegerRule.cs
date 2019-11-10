// <copyright file="IntegerRule.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.Types;

// ------------ error messages I've seen -------------
// error
// error Motor Stop
// error Not Joystick
// error Auto land
namespace Tello
{
    public sealed class IntegerRule : ArgumentRule<int>
    {
        public sealed class Range<T>
            where T : IComparable
        {
            internal Range(T min, T max)
            {
                this.Min = min;
                this.Max = max;
            }

            public readonly T Min;
            public readonly T Max;

            public bool Contains(T value)
            {
                return value.CompareTo(this.Min) >= 0 && value.CompareTo(this.Max) <= 0;
            }
        }

        private readonly Range<int> range;

        internal IntegerRule(Range<int> range)
            : base(Primitive.IsNumeric)
        {
            this.range = range;
        }

        public override bool IsValueAllowed(object value)
        {
            return value is int && this.range.Contains((int)value);
        }
    }
}