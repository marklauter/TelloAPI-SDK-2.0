using System;

namespace Messenger
{
    public abstract class EnvelopeObserver : IObserver<IEnvelope>
    {
        private IDisposable _unsubscriber;

        public EnvelopeObserver()
        {
        }

        public EnvelopeObserver(Receiver receiver)
        {
            Subscribe(receiver);
        }

        public void Subscribe(Receiver receiver)
        {
            if (receiver == null)
            {
                throw new ArgumentNullException(nameof(receiver));
            }

            if (_unsubscriber == null)
            {
                _unsubscriber = receiver.Subscribe(this);
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

        public abstract void OnNext(IEnvelope value);
    }
}
