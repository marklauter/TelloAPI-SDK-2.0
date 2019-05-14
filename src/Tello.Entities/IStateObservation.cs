namespace Tello.Entities
{
    public interface IStateObservation : IObservation
    {
        string Data { get; set; }
    }
}
