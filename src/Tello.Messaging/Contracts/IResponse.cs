using System;

namespace Tello.Controller.Contracts
{
    public interface IResponse
    {
        byte[] Data { get; }
        TimeSpan Elapsed { get; }

        bool IsSuccess { get; }
        string ErrorMessage { get; }
        Exception Exception { get; }
    }
}
