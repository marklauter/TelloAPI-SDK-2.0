using System;

namespace Tello.Controller.Contracts
{
    public interface IRequest
    {
        byte[] Data { get; }
        Guid Id { get; }
    }
}
