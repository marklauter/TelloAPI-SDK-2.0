// <copyright file="Battery.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

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

            this.TemperatureHighC = battery.TemperatureHighC;
            this.TemperatureLowC = battery.TemperatureLowC;
            this.PercentRemaining = battery.PercentRemaining;
            this.Timestamp = battery.Timestamp;
        }

        public Battery(ITelloState state)
        {
            if (state == null)
            {
                throw new ArgumentNullException(nameof(state));
            }

            this.TemperatureLowC = state.TemperatureLowC;
            this.TemperatureHighC = state.TemperatureHighC;
            this.PercentRemaining = state.BatteryPercent;
            this.Timestamp = state.Timestamp;
        }

        public int TemperatureLowC { get; }

        public int TemperatureHighC { get; }

        public int PercentRemaining { get; }

        public DateTime Timestamp { get; }

        public override string ToString()
        {
            return $"B: {this.PercentRemaining} %, TL: {this.TemperatureLowC} C, TH: {this.TemperatureHighC} C";
        }
    }
}
