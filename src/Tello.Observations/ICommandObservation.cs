using Tello.Controller;

namespace Tello.Observations
{
    public interface ICommandObservation : IObservation, ICommandResponseReceivedArgs
    {
    }
}
