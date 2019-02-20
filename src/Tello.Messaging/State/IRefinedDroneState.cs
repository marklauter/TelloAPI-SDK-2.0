namespace Tello.Messaging
{
    public interface IPosition
    {
        int AltitudeAGLInCm { get; set; }
        double AltitudeMSLInCm { get; set; }
        int Heading { get; set; }
        double X { get; set; }
        double Y { get; set; }
    }

    public interface IAttitude
    {
        int Pitch { get; set; }
        int Roll { get; set; }
        int Yaw { get; set; }
    }

    public interface IAirSpeed
    {
        int SpeedX { get; set; }
        int SpeedY { get; set; }
        int SpeedZ { get; set; }
        double AccelerationX { get; set; }
        double AccelerationY { get; set; }
        double AccelerationZ { get; set; }
    }

    public interface IBattery
    {
        int TemperatureLowC { get; set; }
        int TemperatureHighC { get; set; }
        int PercentRemaining { get; set; }
    }

    public interface IHobbsMeter
    {
        int DistanceTraversedInCm { get; set; }
        int MotorTimeInSeconds { get; set; }
    }

    public interface IRefinedDroneState
    {
        IPosition Position { get; set; }
        IAttitude Attitude { get; set; }
        IAirSpeed AirSpeed { get; set; }
        IBattery Battery { get; set; }
        IHobbsMeter HobbsMeter { get; set; }
    }
}
