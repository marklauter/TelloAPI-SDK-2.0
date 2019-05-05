using System;
using Tello.Messaging;

namespace Tello.Observations.Sqlite
{
    public sealed class HobbsMeterObservation : Observation, IHobbsMeter
    {
        public HobbsMeterObservation() : base() { }

        public HobbsMeterObservation(IObservationGroup group, ITelloState state)
            : this(group.Id, state.Timestamp, state.HobbsMeter) { }

        public HobbsMeterObservation(int groupId, ITelloState state)
            : this(groupId, state.Timestamp, state.HobbsMeter) { }

        private HobbsMeterObservation(int groupId, DateTime timestamp, IHobbsMeter hobbsMeter)
            : base(groupId, timestamp)
        {
            DistanceTraversedInCm = hobbsMeter.DistanceTraversedInCm;
            MotorTimeInSeconds = hobbsMeter.MotorTimeInSeconds;
        }

        public int DistanceTraversedInCm { get; set; }

        public int MotorTimeInSeconds { get; set; }

        [SQLite.Ignore]
        public double FlightTimeRemainingInMinutes { get; }
    }
}
