using Tello.Controller;

namespace Tello.Observations
{
    public interface ITelloCommandObservation : IObservation, ICommandResponseReceivedArgs
    {
    }
}
