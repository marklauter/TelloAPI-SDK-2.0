// <copyright file="TelloException.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello
{
    public class TelloException : Exception
    {
        public TelloException()
        {
        }

        public TelloException(string message)
            : base(message)
        {
        }

        public TelloException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
