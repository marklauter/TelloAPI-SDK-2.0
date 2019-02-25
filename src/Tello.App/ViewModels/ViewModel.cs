using System;
using System.Collections.Generic;

namespace Tello.App.ViewModels
{
    public class OpenEventArgs : EventArgs
    {
        public OpenEventArgs() : this(null)
        {
        }

        public OpenEventArgs(Dictionary<string, object> args)
        {
            Args = args != null
                ? new Dictionary<string, object>(args)
                : new Dictionary<string, object>();
        }

        public Dictionary<string, object> Args { get; }
    }

    public class ClosingEventArgs : EventArgs
    {
        public bool CanClose { get; set; }
    }

    public class ViewModel : PropertyChangedNotifier
    {
        public ViewModel(IUIDispatcher dispatcher) : base(dispatcher)
        {
            DisplayName = $"#{GetType().Name}#";
        }

        public string DisplayName { get; set; }

        public void Open(OpenEventArgs args)
        {
            OnOpen(args);
        }

        public bool Close()
        {
            var args = new ClosingEventArgs { CanClose = CanClose };
            if (CanClose)
            {
                OnClosing(args);
            }
            return args.CanClose;
        }

        private bool _canClose = true;
        public bool CanClose { get => _canClose; set => SetProperty(ref _canClose, value); }

        protected virtual void OnOpen(OpenEventArgs args) { }
        protected virtual void OnClosing(ClosingEventArgs args) { }
    }
}
