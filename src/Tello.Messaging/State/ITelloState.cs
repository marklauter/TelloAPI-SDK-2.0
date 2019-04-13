using System;

namespace Tello.Messaging
{
    public interface IPosition
    {
        int AltitudeAGLInCm { get; }
        double AltitudeMSLInCm { get; }
        int Heading { get; }
        double X { get; }
        double Y { get; }
    }

    public interface IAttitude
    {
        int Pitch { get; }
        int Roll { get; }
        int Yaw { get; }
    }

    public interface IAirSpeed
    {
        /// <summary>
        /// lateral airspeed
        /// </summary>
        int SpeedX { get; }

        /// <summary>
        /// forward/backward airspeed
        /// </summary>
        int SpeedY { get; }

        /// <summary>
        /// vertical airspeed
        /// </summary>
        int SpeedZ { get; }

        double AccelerationX { get; }
        double AccelerationY { get; }
        double AccelerationZ { get; }
    }

    public interface IBattery
    {
        int TemperatureLowC { get; }
        int TemperatureHighC { get; }
        int PercentRemaining { get; }
    }

    public interface IHobbsMeter
    {
        int DistanceTraversedInCm { get; }
        int MotorTimeInSeconds { get; }
        double FlightTimeRemainingInMinutes { get; }
    }

    public interface ITelloState
    {
        DateTime Timestamp { get; }
        IPosition Position { get; }
        IAttitude Attitude { get; }
        IAirSpeed AirSpeed { get; }
        IBattery Battery { get; }
        IHobbsMeter HobbsMeter { get; }
    }
}
