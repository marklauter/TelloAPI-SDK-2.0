using Messenger.Simulator;
using Repository.Sqlite;
using System.Threading.Tasks;
using Tello.Demo;

namespace Tello.Simulator.Demo
{
    internal class Program
    {
        private static readonly DroneSimulator _simulator;
        private static IFlightTest _flightTest;

        private enum TestType
        {
            JoyStick,
            WayPoint
        }

        static Program()
        {
            _simulator = new DroneSimulator();
        }

        private static async Task Main(string[] args)
        {
            using (var transceiver = new SimTransceiver(_simulator.MessageHandler))
            using (var stateReceiver = new SimReceiver(_simulator.StateTransmitter))
            using (var videoReceiver = new SimReceiver(_simulator.VideoTransmitter))
            using (var repository = new SqliteRepository((null, "tello.sim.sqlite")))
            {
                var testType = args.Length == 1 && args[0] == "joy"
                    ? TestType.JoyStick
                    : TestType.WayPoint;

                switch (testType)
                {
                    case TestType.JoyStick:
                        _flightTest = new JoyStickFlightTest(
                            repository,
                            transceiver,
                            stateReceiver,
                            videoReceiver);
                        break;

                    case TestType.WayPoint:
                        _flightTest = new CommandFlightTest(
                            repository,
                            transceiver,
                            stateReceiver,
                            videoReceiver);
                        break;

                    default:
                        break;
                }

                await _flightTest.Invoke();
            }
        }
    }
}
