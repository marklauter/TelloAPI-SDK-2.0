// <copyright file="CharacterRule.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tello.Types;

// ------------ error messages I've seen -------------
// error
// error Motor Stop
// error Not Joystick
// error Auto land
namespace Tello
{
    public sealed class CharacterRule : ArgumentRule<char>
    {
        private readonly string allowedValues;

        internal CharacterRule(string allowedValues)
            : base(Primitive.IsString)
        {
            this.allowedValues = allowedValues;
        }

        public override bool IsValueAllowed(object value)
        {
            return (value is char)
                && this.allowedValues.Contains(value.ToString());
        }
    }
}