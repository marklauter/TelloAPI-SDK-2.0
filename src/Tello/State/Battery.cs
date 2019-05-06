using System;

namespace Tello.State
{
    internal sealed class Battery : IBattery
    {
        public Battery(IBattery battery)
        {
            if (battery == null)
            {
                throw new ArgumentNullException(nameof(battery));
            }

            TemperatureHighC = battery.TemperatureHighC;
            TemperatureLowC = battery.TemperatureLowC;
            PercentRemaining = battery.PercentRemaining;
            Timestamp = battery.Timestamp;
        }

        public Battery(ITelloState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            TemperatureLowC = state.TemperatureLowC;
            TemperatureHighC = state.TemperatureHighC;
            PercentRemaining = state.BatteryPercent;
            Timestamp = state.Timestamp;
        }

        public int TemperatureLowC { get; }
        public int TemperatureHighC { get; }
        public int PercentRemaining { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"B: {PercentRemaining} %, TL: {TemperatureLowC} C, TH: {TemperatureHighC} C";
        }
    }
}
