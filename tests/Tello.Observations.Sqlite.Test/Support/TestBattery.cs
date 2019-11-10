// <copyright file="TestBattery.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using Tello.State;

namespace Tello.Entities.Sqlite.Test
{
    public class TestBattery : TestObservation, IBattery
    {
        public int TemperatureLowC { get; } = 1;

        public int TemperatureHighC { get; } = 1;

        public int PercentRemaining { get; } = 1;
    }
}
