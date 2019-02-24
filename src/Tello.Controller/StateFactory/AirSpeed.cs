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

        public AirSpeed(IRawDroneState rawDroneState)
        {
            AccelerationX = rawDroneState.AccelerationX;
            AccelerationY = rawDroneState.AccelerationY;
            AccelerationZ = rawDroneState.AccelerationZ;
            SpeedX = rawDroneState.SpeedX;
            SpeedY = rawDroneState.SpeedY;
            SpeedZ = rawDroneState.SpeedZ;
        }

        public int SpeedX { get; }
        public int SpeedY { get; }
        public int SpeedZ { get; }
        public double AccelerationX { get; }
        public double AccelerationY { get; }
        public double AccelerationZ { get; }

        public override string ToString()
        {
            return $"X: {SpeedX} cm/s, Y: {SpeedY} cm/s, Z: {SpeedZ} cm/s, AX: {AccelerationX} cm/s/s, , AY: {AccelerationY} cm/s/s, , AZ: {AccelerationZ} cm/s/s";
        }
    }
}
