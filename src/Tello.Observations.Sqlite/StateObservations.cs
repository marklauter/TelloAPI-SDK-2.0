using Tello.State;

namespace Tello.Observations.Sqlite
{
    [System.Diagnostics.DebuggerDisplay("{Id}:{GroupId} {Timestamp} - {Data}")]
    public sealed class StateObservation : Observation
    {
        public StateObservation() : base() { }

        public StateObservation(int groupId, ITelloState telloState) 
            : base(groupId, telloState.Timestamp)
        {
            Data = telloState.Data;
        }

        public string Data { get; set; }
    }
}
