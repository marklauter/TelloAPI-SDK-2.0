// <copyright file="InputCommands.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Tello.App.MvvM
{
    public interface IInputCommand : ICommand
    {
        bool Enabled { get; set; }
    }

    public interface IInputCommand<T> : IInputCommand
    {
    }

    public abstract class Command : IInputCommand
    {
        private bool enabled = true;
        private readonly Func<object, bool> canExecute;

        public Command() { }

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

    public class InputCommand : Command
    {
        public InputCommand(Action action)
            : this(action, null) { }

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

    public class InputCommand<T> : Command, IInputCommand<T>
    {
        public InputCommand(Action<T> action)
            : this(action, null) { }

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

    public class AsyncInputCommand : Command
    {
        public AsyncInputCommand(Func<Task> func)
            : this(func, null) { }

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

    public class AsycInputCommand<T> : Command, IInputCommand<T>
    {
        public AsycInputCommand(Func<T, Task> func)
            : this(func, null) { }

        public AsycInputCommand(Func<T, Task> func, Func<object, bool> canExecute)
            : base(canExecute)
        {
            this.func = func ?? throw new ArgumentNullException(nameof(func));
        }

        private readonly Func<T, Task> func;

        public override async void ExecuteInternal(object parameter)
        {
            await this.func((T)parameter);
        }
    }
}
