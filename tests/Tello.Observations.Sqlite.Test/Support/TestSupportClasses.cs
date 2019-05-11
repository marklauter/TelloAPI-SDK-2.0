using System;
using Tello.State;

namespace Tello.Observations.Sqlite.Test
{
    public class TestObservation : IState
    {
        public DateTime Timestamp { get; set; }
    }

    public class TestPosition : TestObservation, IPosition
    {
        public int AltitudeAGLInCm { get; } = 1;
        public double BarometricPressueInCm { get; } = 1;
        public int Heading { get; } = 1;
        public double X { get; } = 1;
        public double Y { get; } = 1;
    }

    public class TestAttitude : TestObservation, IAttitude
    {
        public int Pitch { get; } = 1;
        public int Roll { get; } = 1;
        public int Yaw { get; } = 1;
    }

    public class TestAirSpeed : TestObservation, IAirSpeed
    {
        public int SpeedX { get; } = 1;
        public int SpeedY { get; } = 1;
        public int SpeedZ { get; } = 1;
        public double AccelerationX { get; } = 1;
        public double AccelerationY { get; } = 1;
        public double AccelerationZ { get; } = 1;
    }

    public class TestBattery : TestObservation, IBattery
    {
        public int TemperatureLowC { get; } = 1;
        public int TemperatureHighC { get; } = 1;
        public int PercentRemaining { get; } = 1;
    }

    public class TestHobbsMeter : TestObservation, IHobbsMeter
    {
        public int DistanceTraversedInCm { get; } = 1;
        public int MotorTimeInSeconds { get; } = 1;
        public double FlightTimeRemainingInMinutes => (15 * 60 - MotorTimeInSeconds) / 60.0;
    }
}
