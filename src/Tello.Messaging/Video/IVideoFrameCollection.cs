namespace Tello.Messaging
{
    public interface IVideoFrameCollection : IVideoSample
    {
        int Count { get; }
    }
}
