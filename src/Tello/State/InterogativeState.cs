namespace Tello.State
{
    public sealed class InterogativeState
    {
        public int Speed { get; set; }
        public int Battery { get; set; }
        public int Time { get; set; }
        public string WIFISnr { get; set; }
        public string SdkVersion { get; set; }
        public string SerialNumber { get; set; }
    }
}
