using System;

namespace Messenger
{
    public interface IReceiver<T> : IObservable<IEnvelope<T>>
    {
        void Start();
        void Stop();
    }
}



