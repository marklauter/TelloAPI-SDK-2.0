using System;
using Tello.Messaging;

namespace Tello.Observations.Sqlite
{
    public sealed class BatteryObservation : Observation, IBattery
    {
        public BatteryObservation() : base() { }

        public BatteryObservation(int groupId, ITelloState state)
            : this(groupId, state.Timestamp, state.Battery) { }

        private BatteryObservation(int groupId, DateTime timestamp, IBattery battery)
            : base(groupId, timestamp)
        {
            TemperatureLowC = battery.TemperatureLowC;
            TemperatureHighC = battery.TemperatureHighC;
            PercentRemaining = battery.PercentRemaining;
        }

        public int TemperatureLowC { get; set; }

        public int TemperatureHighC { get; set; }

        public int PercentRemaining { get; set; }
    }
}
