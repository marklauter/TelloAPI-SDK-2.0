// <copyright file="InputCommand.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.App.MvvM
{
    public class InputCommand : Command
    {
        public InputCommand(Action action)
            : this(action, null)
        {
        }

        public InputCommand(Action action, Func<object, bool> canExecute)
            : base(canExecute)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        private readonly Action action;

        public override void ExecuteInternal(object parameter = null)
        {
            this.action();
        }
    }
}
