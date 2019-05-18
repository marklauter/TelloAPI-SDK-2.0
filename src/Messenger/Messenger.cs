using System;
using System.Collections.Concurrent;

namespace Messenger
{
    public class Messenger<T> : IMessenger<T>, IDisposable
    {
        protected void ReponseReceived(IResponse<T> response)
        {
            foreach (var observer in _observers.Keys)
            {
                try
                {
                    observer.OnNext(response);
                }
                catch (Exception ex)
                {
                    ExceptionThrown(ex);
                }
            }
        }

        protected void ExceptionThrown(Exception exception)
        {
            foreach (var observer in _observers.Keys)
            {
                try
                {
                    observer.OnError(exception);
                }
                catch { }
            }
        }

        private readonly ConcurrentDictionary<IObserver<IResponse<T>>, object> _observers = new ConcurrentDictionary<IObserver<IResponse<T>>, object>();

        public IDisposable Subscribe(IObserver<IResponse<T>> observer)
        {
            if (!_observers.ContainsKey(observer))
            {
                _observers[observer] = null;
            }
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<IObserver<IResponse<T>>, object> _observers;
            private readonly IObserver<IResponse<T>> _observer;

            public Unsubscriber(ConcurrentDictionary<IObserver<IResponse<T>>, object> observers, IObserver<IResponse<T>> observer)
            {
                _observers = observers;
                _observer = observer;
            }

            public void Dispose()
            {
                if (_observer != null && _observers.ContainsKey(_observer))
                {
                    while (!_observers.TryRemove(_observer, out _)) { }
                }
            }
        }

        public void Dispose()
        {
            foreach (var observer in _observers.Keys)
            {
                observer.OnCompleted();
            }

            _observers.Clear();
        }
    }
}
