namespace Tello.Messaging
{
    public interface IVideoSampleBuilder : IVideoSample
    {
        void Add(IVideoSample sample);
    }
}
