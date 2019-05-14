using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    public sealed class PositionObservation : Observation, IPosition
    {
        public PositionObservation() : base() { }

        public PositionObservation(
            IObservationGroup group,
            ITelloState state)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  state)
        {
        }

        public PositionObservation(
            int groupId,
            ITelloState state)
            : this(
                  groupId,
                  (state ?? throw new ArgumentNullException(nameof(state))).Timestamp,
                  (state ?? throw new ArgumentNullException(nameof(state))).Position)
        {
        }

        private PositionObservation(
            int groupId,
            DateTime timestamp,
            IPosition position)
            : base(
                  groupId,
                  timestamp)
        {
            if (position == null)
            {
                throw new ArgumentNullException(nameof(position));
            }

            AltitudeAGLInCm = position.AltitudeAGLInCm;
            BarometricPressueInCm = position.BarometricPressueInCm;
            Heading = position.Heading;
            X = position.X;
            Y = position.Y;
        }

        public int AltitudeAGLInCm { get; set; }

        public double BarometricPressueInCm { get; set; }

        public int Heading { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
