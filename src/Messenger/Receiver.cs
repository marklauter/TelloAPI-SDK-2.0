using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace Messenger
{
    //https://docs.microsoft.com/en-us/dotnet/api/system.iobservable-1?view=netstandard-2.0

    public abstract class Receiver : IReceiver, IDisposable
    {
        private CancellationTokenSource _cancellationTokenSource = null;

        public async void Start()
        {
            if (_cancellationTokenSource == null)
            {
                _cancellationTokenSource = new CancellationTokenSource();
                await Task.Run(async () => await Listen(_cancellationTokenSource.Token));
            }
        }

        public void Stop()
        {
            if (_cancellationTokenSource != null 
                && !_cancellationTokenSource.IsCancellationRequested)
            {
                _cancellationTokenSource.Cancel(false);
                _cancellationTokenSource.Dispose();
                _cancellationTokenSource = null;
            }
        }

        protected abstract Task Listen(CancellationToken cancellationToken);

        protected void MessageReceived(IEnvelope message)
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

        private readonly ConcurrentDictionary<IObserver<IEnvelope>, object> _observers = new ConcurrentDictionary<IObserver<IEnvelope>, object>();

        public IDisposable Subscribe(IObserver<IEnvelope> observer)
        {
            if (!_observers.ContainsKey(observer))
            {
                _observers[observer] = null;
            }
            return new Unsubscriber(_observers, observer);
        }

        private class Unsubscriber : IDisposable
        {
            private readonly ConcurrentDictionary<IObserver<IEnvelope>, object> _observers;
            private readonly IObserver<IEnvelope> _observer;

            public Unsubscriber(ConcurrentDictionary<IObserver<IEnvelope>, object> observers, IObserver<IEnvelope> observer)
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
            Stop();

            foreach (var observer in _observers.Keys)
            {
                observer.OnCompleted();
            }

            _observers.Clear();
        }
    }
}
