using System;

namespace Messenger
{
    public interface IMessenger : IObservable<IResponse>
    {
    }
}
