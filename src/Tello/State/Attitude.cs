using System;

namespace Tello.State
{
    internal sealed class Attitude : IAttitude
    {
        public Attitude(IAttitude attitude)
        {
            if (attitude == null)
            {
                throw new ArgumentNullException(nameof(attitude));
            }

            Pitch = attitude.Pitch;
            Roll = attitude.Roll;
            Yaw = attitude.Yaw;
            Timestamp = attitude.Timestamp;
        }

        public Attitude(ITelloState state, bool useMissionPad = false)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            if (!useMissionPad)
            {
                Pitch = state.Pitch;
                Roll = state.Roll;
                Yaw = state.Yaw;
            }
            else
            {
                if (!state.MissionPadDetected)
                {
                    throw new ArgumentException($"{nameof(state)}.{nameof(ITelloState.MissionPadDetected)} == false");
                }

                Pitch = state.MissionPadPitch;
                Roll = state.MissionPadRoll;
                Yaw = state.MissionPadYaw;
            }
            Timestamp = state.Timestamp;
        }

        public int Pitch { get; }
        public int Roll { get; }
        public int Yaw { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"P: {Pitch} deg, R: {Roll} deg, Y: {Yaw} deg";
        }
    }
}
