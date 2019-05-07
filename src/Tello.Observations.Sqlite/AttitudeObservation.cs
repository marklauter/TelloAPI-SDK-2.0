using System;
using Tello.State;

namespace Tello.Observations.Sqlite
{
    public sealed class AttitudeObservation : Observation, IAttitude
    {
        public AttitudeObservation() : base() { }

        public AttitudeObservation(IObservationGroup group, ITelloState state)
            : this(group.Id, state.Timestamp, state.Attitude) { }

        public AttitudeObservation(int groupId, ITelloState state)
            : this(groupId, state.Timestamp, state.Attitude) { }

        private AttitudeObservation(int groupId, DateTime timestamp, IAttitude attitude)
            : base(groupId, timestamp)
        {
            Pitch = attitude.Pitch;
            Roll = attitude.Roll;
            Yaw = attitude.Yaw;
        }

        public int Pitch { get; set; }

        public int Roll { get; set; }

        public int Yaw { get; set; }
    }
}
