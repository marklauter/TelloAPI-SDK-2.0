using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id}:{GroupId} {Timestamp} - {Data}")]
    public sealed class StateObservation : Observation
    {
        public StateObservation()
            : base()
        {
        }

        public StateObservation(
            IObservationGroup group,
            ITelloState telloState)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  telloState)
        {
        }

        public StateObservation(
            int groupId,
            ITelloState telloState)
            : base(
                  groupId,
                  (telloState ?? throw new ArgumentNullException(nameof(telloState))).Timestamp)
        {
            Data = String.IsNullOrEmpty(telloState.Data)
                ? String.Empty
                : telloState.Data;
        }

        public string Data { get; set; }
    }
}
