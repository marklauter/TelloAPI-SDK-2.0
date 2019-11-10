// <copyright file="UINotifier.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using Tello.App.MvvM;

namespace Tello.App.UWP.Services
{
    public class UINotifier : IUINotifier
    {
        public void Error(string message, string title = "error")
        {
            Debug.Write($"{DateTime.Now.TimeOfDay} - {title}: {message}");
        }

        public int Input(string message, string title = "user input required")
        {
            Debug.Write($"{DateTime.Now.TimeOfDay} - {title}: {message}");
            return 0;
        }
    }
}
