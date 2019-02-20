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

        public static Battery FromRawDroneState(IRawDroneState rawDroneState)
        {
            return new Battery
            {
                TemperatureLowC = rawDroneState.TemperatureLowC,
                TemperatureHighC = rawDroneState.TemperatureHighC,
                PercentRemaining = rawDroneState.BatteryPercent,
            };
        }

        public int TemperatureLowC { get; set; }
        public int TemperatureHighC { get; set; }
        public int PercentRemaining { get; set; }

        public override string ToString()
        {
            return $"B: {PercentRemaining} %, TL: {TemperatureLowC} C, TH: {TemperatureHighC} C";
        }
    }
}
