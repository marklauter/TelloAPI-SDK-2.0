using System;
using Tello.Messaging;

namespace Tello.Observations.Sqlite
{
    public sealed class AirSpeedObservation : Observation, IAirSpeed
    {
        public AirSpeedObservation() : base() { }

        public AirSpeedObservation(IObservationGroup group, ITelloState state)
            : this(group.Id, state) { }

        public AirSpeedObservation(int groupId, ITelloState state)
            : this(groupId, state.Timestamp, state.AirSpeed) { }

        private AirSpeedObservation(int groupId, DateTime timestamp, IAirSpeed airspeed)
            : base(groupId, timestamp)
        {
            SpeedX = airspeed.SpeedX;
            SpeedY = airspeed.SpeedY;
            SpeedZ = airspeed.SpeedZ;
            AccelerationX = airspeed.AccelerationX;
            AccelerationY = airspeed.AccelerationY;
            AccelerationZ = airspeed.AccelerationZ;
        }

        public int SpeedX { get; set; }

        public int SpeedY { get; set; }

        public int SpeedZ { get; set; }

        public double AccelerationX { get; set; }

        public double AccelerationY { get; set; }

        public double AccelerationZ { get; set; }
    }
}
