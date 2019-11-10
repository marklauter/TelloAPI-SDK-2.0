// <copyright file="ArgumentRule.cs" company="Mark Lauter">
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
    public abstract class ArgumentRule
    {
        protected ArgumentRule(Type type, Func<object, bool> isTypeAllowed)
        {
            this.Type = type ?? throw new ArgumentNullException(nameof(type));
            this.IsTypeAllowed = isTypeAllowed ?? throw new ArgumentNullException(nameof(isTypeAllowed));
        }

        public readonly Type Type;
        public readonly Func<object, bool> IsTypeAllowed;

        public abstract bool IsValueAllowed(object value);
    }
}