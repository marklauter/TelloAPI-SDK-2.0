using System;
using System.Text;
using System.Threading.Tasks;
using Tello.Messaging;

//https://github.com/dji-sdk/Tello-Python/tree/master/Tello_Video/h264decoder
namespace Tello.Emulator.SDKV2
{
    internal sealed class VideoServer : IMessageRelayService<IVideoSample>, IVideoSampleProvider
    {
        public VideoServer(double frameRate, int secondsToBuffer)
        {
            _frameRate = frameRate;
            _sampleDuration = TimeSpan.FromSeconds(1 / _frameRate);
        }

        private readonly byte[] _sample = Encoding.UTF8.GetBytes("This is fake video sample data.");
        private readonly double _frameRate;
        private readonly TimeSpan _sampleDuration;

        public event EventHandler<MessageReceivedArgs<IVideoSample>> MessageReceived;
        public event EventHandler<MessageRelayExceptionThrownArgs> MessageRelayExceptionThrown;

        public ReceiverStates State { get; private set; }

        private int _frameCount;
        public async void Start()
        {
            if (State != ReceiverStates.Listening)
            {
                State = ReceiverStates.Listening;

                _frameCount = 0;
                await Task.Run(async () =>
                {
                    while (State == ReceiverStates.Listening)
                    {
                        await Task.Delay(_sampleDuration);
                        try
                        {
                            var frame = new VideoFrame(_sample, _frameCount, TimeSpan.FromSeconds(_frameCount / _frameRate), _sampleDuration);
                            var eventArgs = new MessageReceivedArgs<IVideoSample>(frame);
                            MessageReceived?.Invoke(this, eventArgs);
                            ++_frameCount;
                        }
                        catch (Exception ex)
                        {
                            MessageRelayExceptionThrown?.Invoke(this, new MessageRelayExceptionThrownArgs(ex));
                        }
                    }
                });
            }
        }

        public void Stop()
        {
            State = ReceiverStates.Stopped;
        }

        public bool TryGetSample(out IVideoSample sample, TimeSpan timeout)
        {
            sample = State == ReceiverStates.Listening
                ? new VideoFrame(_sample, _frameCount, TimeSpan.FromSeconds(_frameCount / _frameRate), _sampleDuration)
                : null;
            return sample != null;
        }

        public bool TryGetFrame(out IVideoFrame frame, TimeSpan timeout)
        {
            frame = State == ReceiverStates.Listening
                ? new VideoFrame(_sample, _frameCount, TimeSpan.FromSeconds(_frameCount / _frameRate), _sampleDuration)
                : null;
            return frame != null;
        }
    }

    //    internal sealed class VideoServer : UdpServer
    //    {
    //        private int _sampleIndex = 0;
    //        private readonly byte[] _videoData;
    //        private readonly Sample[] _sampleDefs;
    //        private TimeSpan _previousTimeIndex = TimeSpan.FromSeconds(0.0);

    //        public VideoServer(int port, byte[] videoData, Sample[] sampleDefs) : base(port)
    //        {
    //            _videoData = videoData;
    //            _sampleDefs = sampleDefs;
    //        }


    //        private readonly TimeSpan _wait = TimeSpan.FromMilliseconds(2.4);
    //        protected override Task<byte[]> GetDatagram()
    //        {
    //            var sampleDef = _sampleDefs[_sampleIndex];

    //            // sleep to simulate Tello's actual sample rate
    //            //await Task.Delay(TimeSpan.FromMilliseconds(1));
    //            Thread.Sleep(_wait);

    //            // get the video sample 
    //            var sample = new byte[sampleDef.Length];
    //            Array.Copy(_videoData, sampleDef.Offset, sample, 0, sampleDef.Length);

    //            // advance the sample index, wrap at the end
    //            _sampleIndex = (_sampleIndex + 1) % _sampleDefs.Length;

    //            return Task.FromResult(sample);
    //        }
    //    }
}
