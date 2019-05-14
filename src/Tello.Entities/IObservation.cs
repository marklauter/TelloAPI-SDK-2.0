using Repository;
using System;

namespace Tello.Entities
{
    public interface IObservation : IEntity
    {
        int GroupId { get; set; }
        DateTime Timestamp { get; set; }
    }
}
