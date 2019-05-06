using System;

namespace Tello.State
{
    internal sealed class AirSpeed : IAirSpeed
    {
        public AirSpeed(IAirSpeed airspeed)
        {
            if (airspeed == null)
            {
                throw new ArgumentNullException(nameof(airspeed));
            }

            AccelerationX = airspeed.AccelerationX;
            AccelerationY = airspeed.AccelerationY;
            AccelerationZ = airspeed.AccelerationZ;
            SpeedX = airspeed.SpeedX;
            SpeedY = airspeed.SpeedY;
            SpeedZ = airspeed.SpeedZ;
            Timestamp = airspeed.Timestamp;
        }

        public AirSpeed(ITelloState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            AccelerationX = state.AccelerationX;
            AccelerationY = state.AccelerationY;
            AccelerationZ = state.AccelerationZ;
            SpeedX = state.SpeedX;
            SpeedY = state.SpeedY;
            SpeedZ = state.SpeedZ;
            Timestamp = state.Timestamp;
        }

        public int SpeedX { get; }
        public int SpeedY { get; }
        public int SpeedZ { get; }
        public double AccelerationX { get; }
        public double AccelerationY { get; }
        public double AccelerationZ { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"X: {SpeedX} cm/s, Y: {SpeedY} cm/s, Z: {SpeedZ} cm/s, AX: {AccelerationX} cm/s/s, , AY: {AccelerationY} cm/s/s, , AZ: {AccelerationZ} cm/s/s";
        }
    }
}
