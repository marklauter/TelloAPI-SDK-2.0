namespace Tello.Messaging
{
    public interface IDroneState
    {
        int MissionPadId { get; }
        int MissionPadX { get; }
        int MissionPadY { get; }
        int MissionPadZ { get; }
        int MissionPadPitch { get; }
        int MissionPadRoll { get; }
        int MissionPadYaw { get; }

        int Pitch { get; }
        int Roll { get; }
        int Yaw { get; }
        int SpeedX { get; }
        int SpeedY { get; }
        int SpeedZ { get; }
        int TemperatureLowC { get; }
        int TemperatureHighC { get; }
        int DistanceTraversedInCm { get; }
        int HeightInCm { get; }
        int BatteryPercent { get; }
        double BarometerInCm { get; }
        int MotorTimeInSeconds { get; }
        double AccelerationX { get; }
        double AccelerationY { get; }
        double AccelerationZ { get; }

        bool MissionPadDetected { get; }
    }
}
