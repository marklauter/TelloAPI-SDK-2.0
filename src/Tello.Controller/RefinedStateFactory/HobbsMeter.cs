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

        public static HobbsMeter FromRawDroneState(IRawDroneState rawDroneState)
        {
            return new HobbsMeter
            {
                DistanceTraversedInCm = rawDroneState.DistanceTraversedInCm,
                MotorTimeInSeconds = rawDroneState.MotorTimeInSeconds,
            };
        }

        public int DistanceTraversedInCm { get; set; }
        public int MotorTimeInSeconds { get; set; }

        public override string ToString()
        {
            return $"MT: {MotorTimeInSeconds} s, D: {DistanceTraversedInCm} cm";
        }
    }
}
