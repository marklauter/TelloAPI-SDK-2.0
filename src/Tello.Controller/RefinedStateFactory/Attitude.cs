using System;
using Tello.Messaging;

namespace Tello.Controller
{
    internal sealed class Attitude : IAttitude
    {
        public Attitude() { }

        public Attitude(IAttitude attitude)
        {
            Pitch = attitude.Pitch;
            Roll = attitude.Roll;
            Yaw = attitude.Yaw;
        }

        public static Attitude FromRawDroneState(IRawDroneState rawDroneState, bool useMissionPad = false)
        {
            if (useMissionPad)
            {
                if (!rawDroneState.MissionPadDetected)
                {
                    throw new ArgumentException($"{nameof(rawDroneState)}.{nameof(IRawDroneState.MissionPadDetected)} == false");
                }

                return new Attitude
                {
                    Pitch = rawDroneState.MissionPadPitch,
                    Roll = rawDroneState.MissionPadRoll,
                    Yaw = rawDroneState.MissionPadYaw,
                };
            }
            else
            {
                return new Attitude
                {
                    Pitch = rawDroneState.Pitch,
                    Roll = rawDroneState.Roll,
                    Yaw = rawDroneState.Yaw,
                };
            }
        }

        public int Pitch { get; set; }
        public int Roll { get; set; }
        public int Yaw { get; set; }

        public override string ToString()
        {
            return $"P: {Pitch} deg, R: {Roll} deg, Y: {Yaw} deg";
        }
    }
}
