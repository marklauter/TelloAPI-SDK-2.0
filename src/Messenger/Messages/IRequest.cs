using System;

namespace Messenger
{
    public interface IRequest: IMessage
    {
        /// <summary>
        /// some requests take longer than others, so this makes it possible to set per-request timeout
        /// </summary>
        TimeSpan Timeout { get; }
    }

    public interface IRequest<T> : IRequest, IMessage<T>
    {
    }
}
