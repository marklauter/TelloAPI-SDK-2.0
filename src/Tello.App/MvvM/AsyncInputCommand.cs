// <copyright file="AsyncInputCommand.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;

namespace Tello.App.MvvM
{
    public class AsyncInputCommand : Command
    {
        public AsyncInputCommand(Func<Task> func)
            : this(func, null)
        {
        }

        public AsyncInputCommand(Func<Task> func, Func<object, bool> canExecute)
            : base(canExecute)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
        }

        private readonly Func<Task> func;

        public override async void ExecuteInternal(object parameter = null)
        {
            await this.func();
        }
    }
}
