using Repository;
using System;

namespace Tello.Observations
{
    public interface ISession : IEntity
    {
        DateTime StartTime { get; set; }
        TimeSpan Duration { get; set; }
    }
}
