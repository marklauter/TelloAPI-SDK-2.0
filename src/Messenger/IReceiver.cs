using System;

namespace Messenger
{
    public interface IReceiver : IObservable<IEnvelope>
    {
        void Start();
        void Stop();
    }
}



