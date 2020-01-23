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
        private static readonly DroneSimulator Simulator;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:Field names should begin with lower-case letter", Justification = "Conflicts with Microsoft naming guidelines.")]
        private static IFlightTest FlightTest;

        private enum TestType
        {
            JoyStick,
            WayPoint,
        }

        static Program()
        {
            Simulator = new DroneSimulator();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Main.")]
        private static async Task Main(string[] args)
        {
            using (var transceiver = new SimTransceiver(Simulator.MessageHandler))
            using (var stateReceiver = new SimReceiver(Simulator.StateTransmitter))
            using (var videoReceiver = new SimReceiver(Simulator.VideoTransmitter))
            using (var repository = new SqliteRepository((null, "tello.sim.sqlite")))
            {
                var testType = args.Length == 1 && args[0] == "joy"
                    ? TestType.JoyStick
                    : TestType.WayPoint;

                switch (testType)
                {
                    case TestType.JoyStick:
                        FlightTest = new JoyStickFlightTest(
                            repository,
                            transceiver,
                            stateReceiver,
                            videoReceiver);
                        break;

                    case TestType.WayPoint:
                        FlightTest = new CommandFlightTest(
                            repository,
                            transceiver,
                            stateReceiver,
                            videoReceiver);
                        break;

                    default:
                        break;
                }

                await FlightTest.Invoke();
            }
        }
    }
}
