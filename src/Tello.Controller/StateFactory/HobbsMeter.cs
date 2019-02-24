using Tello.Messaging;

namespace Tello.Controller
{
    internal sealed class HobbsMeter : IHobbsMeter
    {
        public HobbsMeter() { }

        public HobbsMeter(IHobbsMeter hobbsMeter)
        {
            DistanceTraversedInCm = hobbsMeter.DistanceTraversedInCm;
            MotorTimeInSeconds = hobbsMeter.MotorTimeInSeconds;
        }

        public HobbsMeter(IRawDroneState rawDroneState)
        {
            DistanceTraversedInCm = rawDroneState.DistanceTraversedInCm;
            MotorTimeInSeconds = rawDroneState.MotorTimeInSeconds;
        }

        public int DistanceTraversedInCm { get; }
        public int MotorTimeInSeconds { get; }

        public override string ToString()
        {
            return $"MT: {MotorTimeInSeconds} s, D: {DistanceTraversedInCm} cm";
        }
    }
}
