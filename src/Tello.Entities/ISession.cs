using Repository;
using System;

namespace Tello.Entities
{
    public interface ISession : IEntity
    {
        DateTime StartTime { get; set; }
        TimeSpan Duration { get; set; }
    }
}
