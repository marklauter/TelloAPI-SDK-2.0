using System;
using Tello.Messaging;

namespace Tello.Observations.Sqlite
{
    public sealed class TelloStateObservation : Observation
    {
        public TelloStateObservation() : base() { }

        public TelloStateObservation(ITelloState telloState, string groupId) : base(groupId)
        {
            Timestamp = telloState.Timestamp;
            Data = telloState.Data;
        }

        public string Data { get; set; }
    }

    public sealed class PositionObservation : Observation, IPosition
    {
        public PositionObservation() : base() { }

        private PositionObservation(IPosition position, string groupId) : base(groupId)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }
            AltitudeAGLInCm = position.AltitudeAGLInCm;
            AltitudeMSLInCm = position.AltitudeMSLInCm;
            Heading = position.Heading;
            X = position.X;
            Y = position.Y;
        }

        public PositionObservation(ITelloState telloState, string groupId) : this(telloState?.Position, groupId)
        {
            Timestamp = telloState.Timestamp;
        }

        public int AltitudeAGLInCm { get; set; }

        public double AltitudeMSLInCm { get; set; }

        public int Heading { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }

    public sealed class AttitudeObservation : Observation, IAttitude
    {
        public AttitudeObservation() : base() { }

        private AttitudeObservation(IAttitude attitude, string groupId) : base(groupId)
        {
            if (attitude == null)
            {
                throw new ArgumentNullException(nameof(attitude));
            }
            Pitch = attitude.Pitch;
            Roll = attitude.Roll;
            Yaw = attitude.Yaw;
        }

        public AttitudeObservation(ITelloState telloState, string groupId) : this(telloState?.Attitude, groupId)
        {
            Timestamp = telloState.Timestamp;
        }
        public int Pitch { get; set; }

        public int Roll { get; set; }

        public int Yaw { get; set; }
    }

    public sealed class AirSpeedObservation : Observation, IAirSpeed
    {
        public AirSpeedObservation() : base() { }

        private AirSpeedObservation(IAirSpeed airspeed, string groupId) : base(groupId)
        {
            if (airspeed == null)
            {
                throw new ArgumentNullException(nameof(airspeed));
            }
            SpeedX = airspeed.SpeedX;
            SpeedY = airspeed.SpeedY;
            SpeedZ = airspeed.SpeedZ;
            AccelerationX = airspeed.AccelerationX;
            AccelerationY = airspeed.AccelerationY;
            AccelerationZ = airspeed.AccelerationZ;
        }

        public AirSpeedObservation(ITelloState telloState, string groupId) : this(telloState?.AirSpeed, groupId)
        {
            Timestamp = telloState.Timestamp;
        }

        public int SpeedX { get; set; }

        public int SpeedY { get; set; }

        public int SpeedZ { get; set; }

        public double AccelerationX { get; set; }

        public double AccelerationY { get; set; }

        public double AccelerationZ { get; set; }
    }

    public sealed class BatteryObservation : Observation, IBattery
    {
        public BatteryObservation() : base() { }

        private BatteryObservation(IBattery battery, string groupId) : base(groupId)
        {
            if (battery == null)
            {
                throw new ArgumentNullException(nameof(battery));
            }
            TemperatureLowC = battery.TemperatureLowC;
            TemperatureHighC = battery.TemperatureHighC;
            PercentRemaining = battery.PercentRemaining;
        }

        public BatteryObservation(ITelloState telloState, string groupId) : this(telloState?.Battery, groupId)
        {
            Timestamp = telloState.Timestamp;
        }

        public int TemperatureLowC { get; set; }

        public int TemperatureHighC { get; set; }

        public int PercentRemaining { get; set; }
    }

    public sealed class HobbsMeterObservation : Observation, IHobbsMeter
    {
        public HobbsMeterObservation() : base() { }

        private HobbsMeterObservation(IHobbsMeter hobbsMeter, string groupId) : base(groupId)
        {
            if (hobbsMeter == null)
            {
                throw new ArgumentNullException(nameof(hobbsMeter));
            }
            DistanceTraversedInCm = hobbsMeter.DistanceTraversedInCm;
            MotorTimeInSeconds = hobbsMeter.MotorTimeInSeconds;
        }

        public HobbsMeterObservation(ITelloState telloState, string groupId) : this(telloState?.HobbsMeter, groupId)
        {
            Timestamp = telloState.Timestamp;
        }

        public int DistanceTraversedInCm { get; set; }

        public int MotorTimeInSeconds { get; set; }

        [SQLite.Ignore]
        public double FlightTimeRemainingInMinutes { get; }
    }
}
