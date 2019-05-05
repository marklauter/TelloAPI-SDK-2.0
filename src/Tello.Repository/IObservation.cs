using Repository;
using System;

namespace Tello.Observations
{
    public interface IObservation : IEntity
    {
        int SessionId { get; set; }
        DateTime Timestamp { get; set; }
    }
}
