using Messenger.Udp;
using Repository.Sqlite;
using System.Net;
using System.Threading.Tasks;
using Tello.Demo;

namespace Tello.Udp.Demo
{
    internal class Program
    {
        private static readonly FlightTest _flightTest;

        static Program()
        {
            var transceiver = new UdpTransceiver(IPAddress.Parse("192.168.10.1"), 8889);
            var stateReceiver = new UdpReceiver(8890);
            var videoReceiver = new UdpReceiver(11111);

            var repository = new SqliteRepository((null, "tello.udp.sqlite"));

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
