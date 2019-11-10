// <copyright file="BatteryObservation.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Tello.State;

namespace Tello.Entities.Sqlite
{
    public sealed class BatteryObservation : Observation, IBattery
    {
        public BatteryObservation()
            : base()
        {
        }

        public BatteryObservation(
            IObservationGroup group,
            ITelloState state)
            : this(
                  (group ?? throw new ArgumentNullException(nameof(group))).Id,
                  state.Timestamp,
                  state.Battery)
        {
        }

        public BatteryObservation(
            int groupId,
            ITelloState state)
            : this(
                  groupId,
                  state.Timestamp,
                  state.Battery)
        {
        }

        private BatteryObservation(
            int groupId,
            DateTime timestamp,
            IBattery battery)
            : base(
                  groupId,
                  timestamp)
        {
            if (battery == null)
            {
                throw new ArgumentNullException(nameof(battery));
            }

            this.TemperatureLowC = battery.TemperatureLowC;
            this.TemperatureHighC = battery.TemperatureHighC;
            this.PercentRemaining = battery.PercentRemaining;
        }

        public int TemperatureLowC { get; set; }

        public int TemperatureHighC { get; set; }

        public int PercentRemaining { get; set; }
    }
}
