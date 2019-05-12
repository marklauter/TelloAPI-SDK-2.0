using System;

namespace Messenger
{
    public abstract class Observer<T> : IObserver<T>
    {
        private IDisposable _unsubscriber;

        public Observer() { }

        public Observer(IObservable<T> observable)
        {
            Subscribe(observable);
        }

        public void Subscribe(IObservable<T> observable)
        {
            if (observable == null)
            {
                throw new ArgumentNullException(nameof(observable));
            }

            if (_unsubscriber == null)
            {
                _unsubscriber = observable.Subscribe(this);
            }
        }

        public void Unsubscribe()
        {
            if (_unsubscriber != null)
            {
                _unsubscriber.Dispose();
                _unsubscriber = null;
            }
        }

        public void OnCompleted()
        {
            Unsubscribe();
        }

        public abstract void OnError(Exception error);

        public abstract void OnNext(T message);
    }
}
