using System;

namespace Tello.Messaging
{
    public interface IRequest
    {
        byte[] Data { get; }
        Guid Id { get; }
        TimeSpan Timeout { get; }
    }
}
