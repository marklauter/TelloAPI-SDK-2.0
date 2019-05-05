namespace Tello.Observations
{
    public interface IStateObservation : IObservation
    {
        string Data { get; set; }
    }
}
