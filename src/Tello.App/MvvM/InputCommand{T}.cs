// <copyright file="InputCommand{T}.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.App.MvvM
{
    public class InputCommand<T> : Command, IInputCommand<T>
    {
        public InputCommand(Action<T> action)
            : this(action, null)
        {
        }

        public InputCommand(Action<T> action, Func<object, bool> canExecute)
            : base(canExecute)
        {
            this.action = action ?? throw new ArgumentNullException(nameof(action));
        }

        private readonly Action<T> action;

        public override void ExecuteInternal(object parameter)
        {
            this.action((T)Convert.ChangeType(parameter, typeof(T)));
        }
    }
}
