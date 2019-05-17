using Messenger.Simulator;
using Repository.Sqlite;
using System.Threading.Tasks;
using Tello.Demo;

namespace Tello.Simulator.Demo
{
    internal class Program
    {
        private static readonly FlightTest _flightTest;
        private static readonly DroneSimulator _simulator;

        static Program()
        {
            _simulator = new DroneSimulator();
            var transceiver = new SimTransceiver(_simulator.MessageHandler);
            var stateReceiver = new SimReceiver(_simulator.StateTransmitter);
            var videoReceiver = new SimReceiver(_simulator.VideoTransmitter);

            var repository = new SqliteRepository((null, "tello.sim.sqlite"));

            _flightTest = new FlightTest(
                repository,
                transceiver,
                stateReceiver,
                videoReceiver);
        }

        private static async Task Main(string[] _)
        {
            await _flightTest.Invoke();
        }
    }
}
