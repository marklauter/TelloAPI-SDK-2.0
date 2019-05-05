using Tello.Messaging;

namespace Tello.Observations.Sqlite.Test
{
    public class TestPosition : IPosition
    {
        public int AltitudeAGLInCm { get; } = 1;
        public double AltitudeMSLInCm { get; } = 1;
        public int Heading { get; } = 1;
        public double X { get; } = 1;
        public double Y { get; } = 1;
    }

    public class TestAttitude : IAttitude
    {
        public int Pitch { get; } = 1;
        public int Roll { get; } = 1;
        public int Yaw { get; } = 1;
    }

    public class TestAirSpeed : IAirSpeed
    {
        public int SpeedX { get; } = 1;
        public int SpeedY { get; } = 1;
        public int SpeedZ { get; } = 1;
        public double AccelerationX { get; } = 1;
        public double AccelerationY { get; } = 1;
        public double AccelerationZ { get; } = 1;
    }

    public class TestBattery : IBattery
    {
        public int TemperatureLowC { get; } = 1;
        public int TemperatureHighC { get; } = 1;
        public int PercentRemaining { get; } = 1;
    }

    public class TestHobbsMeter : IHobbsMeter
    {
        public int DistanceTraversedInCm { get; } = 1;
        public int MotorTimeInSeconds { get; } = 1;
        public double FlightTimeRemainingInMinutes => (15 * 60 - MotorTimeInSeconds) / 60.0;
    }
}
