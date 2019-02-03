namespace Tello.Messaging
{
    public interface IVideoFrame : IVideoSample
    {
        int Index { get; }
    }
}
