using Repository;

namespace Tello.Observations
{
    public interface IObservationGroup : IEntity
    {
        int SessionId { get; set; }
    }
}
