using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    public sealed class AttitudeObservation : Observation, IAttitude
    {
        public AttitudeObservation() : base() { }

        public AttitudeObservation(
            IObservationGroup group,
            ITelloState state)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).Attitude)
        { }

        public AttitudeObservation(
            int groupId,
            ITelloState state)
            : this(
                  groupId,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).Attitude)
        { }

        private AttitudeObservation(
            int groupId,
            DateTime timestamp,
            IAttitude attitude)
            : base(
                  groupId,
                  timestamp)
        {
            if (attitude == null)
            {
                throw new ArgumentNullException(nameof(attitude));
            }

            Pitch = attitude.Pitch;
            Roll = attitude.Roll;
            Yaw = attitude.Yaw;
        }

        public int Pitch { get; set; }

        public int Roll { get; set; }

        public int Yaw { get; set; }
    }
}
