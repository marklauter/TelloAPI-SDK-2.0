using System;

namespace Tello.Emulator.SDKV2
{
    public sealed class Sample
    {
        public bool IsFrameStart { get; set; }
        public long Offset { get; set; }
        public int Length { get; set; }
        public TimeSpan TimeIndex { get; set; }
    }
}
