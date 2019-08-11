using Messenger.Udp;
using Repository.Sqlite;
using System.Net;
using System.Threading.Tasks;
using Tello.Demo;

namespace Tello.Udp.Demo
{
    internal class Program
    {
        private static IFlightTest _flightTest;

        private enum TestType
        {
            JoyStick,
            WayPoint
        }

        private static async Task Main(string[] args)
        {
            using (var transceiver = new UdpTransceiver(IPAddress.Parse("192.168.10.1"), 8889))
            using (var stateReceiver = new UdpReceiver(8890))
            using (var videoReceiver = new UdpReceiver(11111))
            using (var repository = new SqliteRepository((null, "tello.udp.sqlite")))
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
                        _flightTest = new JoyStickFlightTest(
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
