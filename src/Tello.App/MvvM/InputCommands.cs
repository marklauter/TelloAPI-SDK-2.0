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
        private bool _enabled = true;
        private readonly Func<object, bool> _canExecute;

        public Command() { }

        public Command(Func<object, bool> canExecute)
        {
            _canExecute = canExecute;
        }

        public event EventHandler CanExecuteChanged;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    CanExecuteChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public bool CanExecute(object parameter = null)
        {
            return _canExecute != null
                ? _canExecute.Invoke(parameter)
                : Enabled;
        }

        public void Execute(object paramter)
        {
            if (CanExecute(paramter))
            {
                var enabled = Enabled;
                Enabled = false;
                try
                {
                    ExecuteInternal(paramter);
                }
                finally
                {
                    Enabled = enabled;
                }
            }
        }

        public abstract void ExecuteInternal(object parameter);
    }

    public class InputCommand : Command
    {
        public InputCommand(Action action) : this(action, null) { }

        public InputCommand(Action action, Func<object, bool> canExecute) : base(canExecute)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        private readonly Action _action;

        public override void ExecuteInternal(object parameter = null)
        {
            _action();
        }
    }

    public class InputCommand<T> : Command, IInputCommand<T>
    {
        public InputCommand(Action<T> action) : this(action, null) { }

        public InputCommand(Action<T> action, Func<object, bool> canExecute) : base(canExecute)
        {
            _action = action ?? throw new ArgumentNullException(nameof(action));
        }

        private readonly Action<T> _action;

        public override void ExecuteInternal(object parameter)
        {
            _action((T)parameter);
        }
    }

    public class AsyncInputCommand : Command
    {
        public AsyncInputCommand(Func<Task> func) : this(func, null) { }

        public AsyncInputCommand(Func<Task> func, Func<object, bool> canExecute) : base(canExecute)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        private readonly Func<Task> _func;

        public override async void ExecuteInternal(object parameter = null)
        {
            await _func();
        }
    }

    public class AsycInputCommand<T> : Command, IInputCommand<T>
    {
        public AsycInputCommand(Func<T, Task> func) : this(func, null) { }

        public AsycInputCommand(Func<T, Task> func, Func<object, bool> canExecute) : base(canExecute)
        {
            _func = func ?? throw new ArgumentNullException(nameof(func));
        }

        private readonly Func<T, Task> _func;

        public override async void ExecuteInternal(object parameter)
        {
            await _func((T)parameter);
        }
    }
}
