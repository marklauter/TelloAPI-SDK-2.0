using System;

namespace Tello.App.MvvM
{
    public interface IUIDispatcher
    {
        void Invoke(Action action);
        void Invoke(Action<object> action, object state);
    }
}
