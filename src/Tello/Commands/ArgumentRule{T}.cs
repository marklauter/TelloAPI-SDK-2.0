// <copyright file="ArgumentRule{T}.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

// ------------ error messages I've seen -------------
// error
// error Motor Stop
// error Not Joystick
// error Auto land
namespace Tello
{
    public abstract class ArgumentRule<T> : ArgumentRule
    {
        internal ArgumentRule(Func<object, bool> isTypeAllowed)
            : base(typeof(T), isTypeAllowed)
        {
        }
    }
}