using System;

namespace Tello.Messaging
{
    public interface IResponse
    {
        byte[] Data { get; }
        TimeSpan TimeElapsed { get; }
        Guid RequestId { get; }

        bool IsSuccess { get; }
        string ErrorMessage { get; }
        Exception Exception { get; }
    }
}
