// <copyright file="OpenEventArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;

namespace Tello.App.MvvM
{
    public class OpenEventArgs : EventArgs
    {
        public OpenEventArgs()
            : this(null)
        {
        }

        public OpenEventArgs(Dictionary<string, object> args)
        {
            this.Args = args != null
                ? new Dictionary<string, object>(args)
                : new Dictionary<string, object>();
        }

        public Dictionary<string, object> Args { get; }
    }
}
