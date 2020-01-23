// <copyright file="StringRule.cs" company="Mark Lauter">
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
    public sealed class StringRule : ArgumentRule<string>
    {
        internal StringRule()
            : base(Primitive.IsString)
        {
        }

        public override bool IsValueAllowed(object value)
        {
            return value != null
                && ((string)value).Length > 0;
        }
    }
}