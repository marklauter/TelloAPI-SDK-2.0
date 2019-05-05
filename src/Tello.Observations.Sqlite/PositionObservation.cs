using System;
using Tello.Messaging;

namespace Tello.Observations.Sqlite
{
    public sealed class PositionObservation : Observation, IPosition
    {
        public PositionObservation() : base() { }

        public PositionObservation(IObservationGroup group, ITelloState state)
            : this(group.Id, state) { }

        public PositionObservation(int groupId, ITelloState state)
            : this(groupId, state.Timestamp, state.Position) { }

        private PositionObservation(int groupId, DateTime timestamp, IPosition position)
            : base(groupId, timestamp)
        {
            AltitudeAGLInCm = position.AltitudeAGLInCm;
            AltitudeMSLInCm = position.AltitudeMSLInCm;
            Heading = position.Heading;
            X = position.X;
            Y = position.Y;
        }

        public int AltitudeAGLInCm { get; set; }

        public double AltitudeMSLInCm { get; set; }

        public int Heading { get; set; }

        public double X { get; set; }

        public double Y { get; set; }
    }
}
