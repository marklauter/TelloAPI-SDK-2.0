// <copyright file="ClosingEventArgs.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.App.MvvM
{
    public class ClosingEventArgs : EventArgs
    {
        public bool CanClose { get; set; }
    }
}
