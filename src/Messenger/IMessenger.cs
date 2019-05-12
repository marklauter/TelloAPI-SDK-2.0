using System;

namespace Messenger
{
    public interface IMessenger<T> : IObservable<IResponse<T>>
    {
    }
}
