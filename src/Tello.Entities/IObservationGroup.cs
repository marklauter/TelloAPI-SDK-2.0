using Repository;

namespace Tello.Entities
{
    public interface IObservationGroup : IEntity
    {
        int SessionId { get; set; }
    }
}
