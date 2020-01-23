// <copyright file="IUINotifier.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tello.App.MvvM
{
    public interface IUINotifier
    {
        void Error(string message, string title = "error");

        int Input(string message, string title = "user input required");
    }
}
