using System;
using System.Collections.Concurrent;

namespace Messenger
{
    public class Messenger : IMessenger
    {
        protected void ReponseReceived(IResponse message)
        {
            foreach (var observer in _observers.Keys)
            {
                observer.OnNext(message);
            }
        }

        protected void ExceptionThrown(Exception exception)
        {
            foreach (var observer in _observers.Keys)
            {
                observer.OnError(exception);
            }
        }

        private readonly ConcurrentDictionary<IObserver<IResponse>, object> _observers = new ConcurrentDictionary<IObserver<IResponse>, object>();

        public IDisposable Subscribe(IObserver<IResponse> observer)
        {
            if (!_observers.ContainsKey(observer))
            {
                _observers[observer] = null;
            }
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<IObserver<IResponse>, object> _observers;
            private readonly IObserver<IResponse> _observer;

            public Unsubscriber(ConcurrentDictionary<IObserver<IResponse>, object> observers, IObserver<IResponse> observer)
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
