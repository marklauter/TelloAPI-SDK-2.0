using Tello.Messaging;

namespace Tello.Controller
{
    internal sealed class Battery : IBattery
    {
        public Battery() { }

        public Battery(IBattery battery)
        {
            TemperatureHighC = battery.TemperatureHighC;
            TemperatureLowC = battery.TemperatureLowC;
            PercentRemaining = battery.PercentRemaining;
        }

        public Battery(IRawDroneState rawDroneState)
        {
            TemperatureLowC = rawDroneState.TemperatureLowC;
            TemperatureHighC = rawDroneState.TemperatureHighC;
            PercentRemaining = rawDroneState.BatteryPercent;
        }

        public int TemperatureLowC { get; }
        public int TemperatureHighC { get; }
        public int PercentRemaining { get; }

        public override string ToString()
        {
            return $"B: {PercentRemaining} %, TL: {TemperatureLowC} C, TH: {TemperatureHighC} C";
        }
    }
}
