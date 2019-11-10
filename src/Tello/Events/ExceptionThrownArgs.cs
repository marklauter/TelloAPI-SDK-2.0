// <copyright file="ExceptionThrownArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.Events
{
    public sealed class ExceptionThrownArgs : EventArgs
    {
        public ExceptionThrownArgs(TelloException exception)
        {
            this.Exception = exception ?? throw new ArgumentNullException(nameof(exception));
        }

        public TelloException Exception { get; }
    }
}
