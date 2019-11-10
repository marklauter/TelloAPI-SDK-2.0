// <copyright file="Program.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System.Threading.Tasks;
using Messenger.Simulator;
using Repository.Sqlite;
using Tello.Demo;

namespace Tello.Simulator.Demo
{
    internal class Program
    {
        private static readonly DroneSimulator simulator;
        private static IFlightTest flightTest;

        private enum TestType
        {
            JoyStick,
            WayPoint
        }

        static Program()
        {
            simulator = new DroneSimulator();
        }

        private static async Task Main(string[] args)
        {
            using (var transceiver = new SimTransceiver(simulator.MessageHandler))
            using (var stateReceiver = new SimReceiver(simulator.StateTransmitter))
            using (var videoReceiver = new SimReceiver(simulator.VideoTransmitter))
            using (var repository = new SqliteRepository((null, "tello.sim.sqlite")))
            {
                var testType = args.Length == 1 && args[0] == "joy"
                    ? TestType.JoyStick
                    : TestType.WayPoint;

                switch (testType)
                {
                    case TestType.JoyStick:
                        flightTest = new JoyStickFlightTest(
                            repository,
                            transceiver,
                            stateReceiver,
                            videoReceiver);
                        break;

                    case TestType.WayPoint:
                        flightTest = new CommandFlightTest(
                            repository,
                            transceiver,
                            stateReceiver,
                            videoReceiver);
                        break;

                    default:
                        break;
                }

                await flightTest.Invoke();
            }
        }
    }
}
