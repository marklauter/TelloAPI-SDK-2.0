using Tello.Messaging;

namespace Tello.Controller
{
    internal sealed class AirSpeed : IAirSpeed
    {
        public AirSpeed() { }

        public AirSpeed(IAirSpeed airSpeed)
        {
            SpeedX = airSpeed.SpeedX;
            SpeedY = airSpeed.SpeedY;
            SpeedZ = airSpeed.SpeedZ;
            AccelerationX = airSpeed.AccelerationX;
            AccelerationY = airSpeed.AccelerationY;
            AccelerationZ = airSpeed.AccelerationZ;
        }

        public static AirSpeed FromRawDroneState(IRawDroneState rawDroneState)
        {
            return new AirSpeed
            {
                AccelerationX = rawDroneState.AccelerationX,
                AccelerationY = rawDroneState.AccelerationY,
                AccelerationZ = rawDroneState.AccelerationZ,
                SpeedX = rawDroneState.SpeedX,
                SpeedY = rawDroneState.SpeedY,
                SpeedZ = rawDroneState.SpeedZ,
            };
        }

        public int SpeedX { get; set; }
        public int SpeedY { get; set; }
        public int SpeedZ { get; set; }
        public double AccelerationX { get; set; }
        public double AccelerationY { get; set; }
        public double AccelerationZ { get; set; }

        public override string ToString()
        {
            return $"X: {SpeedX} cm/s, Y: {SpeedY} cm/s, Z: {SpeedZ} cm/s, AX: {AccelerationX} cm/s/s, , AY: {AccelerationY} cm/s/s, , AZ: {AccelerationZ} cm/s/s";
        }
    }
}
