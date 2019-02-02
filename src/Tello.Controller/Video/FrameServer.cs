using System;
using Tello.Udp;

namespace Tello.Controller.Video
{
    internal sealed class FrameServer
    {
        //public VideoFrameServer(double frameRate, int bitRate, TimeSpan bufferTime, int bytesPerSample = 1460, int port = 11111)
        public FrameServer(double frameRate, TimeSpan bufferTime, int port = 11111)
        {
            _frameComposer = new FrameComposer(frameRate, bufferTime);
            _frameComposer.FrameReady += _frameComposer_FrameReady;

            _listener = new Listener(port);
            _listener.DatagramReceived += _udpReceiver_DatagramReceived;
            _listener.Start();
        }

        private readonly FrameComposer _frameComposer;
        private readonly Listener _listener;

        public event EventHandler<FrameReadyArgs> FrameReady;

        private void _udpReceiver_DatagramReceived(object sender, DatagramReceivedArgs e)
        {
            _frameComposer.AddSample(e.Datagram);
        }

        private void _frameComposer_FrameReady(object sender, FrameReadyArgs e)
        {
            FrameReady?.Invoke(this, e);
        }

        public FrameCollection GetSample(TimeSpan timeout)
        {
            return _frameComposer.GetFrames(timeout);
        }

        public bool TryReadFrame(out Frame frame, TimeSpan timeout)
        {
            return _frameComposer.TryGetFrame(out frame, timeout);
        }
    }
}
