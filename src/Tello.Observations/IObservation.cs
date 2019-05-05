using Repository;
using System;

namespace Tello.Observations
{
    public interface IObservation : IEntity
    {
        int GroupId { get; set; }
        DateTime Timestamp { get; set; }
    }
}
