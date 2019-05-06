using System;

namespace Tello.State
{
    internal sealed class HobbsMeter : IHobbsMeter
    {
        public HobbsMeter(IHobbsMeter hobbsMeter)
        {
            if (hobbsMeter == null)
            {
                throw new ArgumentNullException(nameof(hobbsMeter));
            }

            DistanceTraversedInCm = hobbsMeter.DistanceTraversedInCm;
            MotorTimeInSeconds = hobbsMeter.MotorTimeInSeconds;
            Timestamp = hobbsMeter.Timestamp;
        }

        public HobbsMeter(ITelloState state)
        {
            DistanceTraversedInCm = state.DistanceTraversedInCm;
            MotorTimeInSeconds = state.MotorTimeInSeconds;
            Timestamp = state.Timestamp;
        }

        public int DistanceTraversedInCm { get; }
        public int MotorTimeInSeconds { get; }

        // documentation says there's ~ 15 minutes of battery
        public double FlightTimeRemainingInMinutes => (15 * 60 - MotorTimeInSeconds) / 60.0;

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"MT: {MotorTimeInSeconds} s, D: {DistanceTraversedInCm} cm";
        }
    }
}
