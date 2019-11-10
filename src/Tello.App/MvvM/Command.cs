// <copyright file="Command.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.App.MvvM
{
    public abstract class Command : IInputCommand
    {
        private bool enabled = true;
        private readonly Func<object, bool> canExecute;

        public Command()
        {
        }

        public Command(Func<object, bool> canExecute)
        {
            this.canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool Enabled
        {
            get => this.enabled;
            set
            {
                if (this.enabled != value)
                {
                    this.enabled = value;
                    this.CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter = null)
        {
            return this.canExecute != null
                ? this.canExecute.Invoke(parameter)
                : this.Enabled;
        }

        public void Execute(object paramter)
        {
            if (this.CanExecute(paramter))
            {
                var enabled = this.Enabled;
                this.Enabled = false;
                try
                {
                    this.ExecuteInternal(paramter);
                }
                finally
                {
                    this.Enabled = enabled;
                }
            }
        }

        public abstract void ExecuteInternal(object parameter);
    }
}
