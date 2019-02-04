namespace Tello.Messaging
{
    public interface IVideoFrame : IVideoSample
    {
        int FrameIndex { get; }
    }
}
