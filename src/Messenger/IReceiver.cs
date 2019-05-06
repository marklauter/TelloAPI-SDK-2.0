using System;

namespace Messenger
{
    public interface IReceiver<T> : IObservable<IMessage<T>>
    {
        void Start();
        void Stop();
    }
}



