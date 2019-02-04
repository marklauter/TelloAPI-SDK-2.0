using Newtonsoft.Json;
using System;
using System.Diagnostics;
using Tello.Controller;

namespace Tello.Emulator.SDKV2.Demo
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var tello = new TelloEmulator();
            var controller = new FlightController(tello, tello.StateServer, tello.VideoServer);
            controller.DroneStateReceived += Controller_DroneStateReceived;
            controller.FlightControllerExceptionThrown += Controller_FlightControllerExceptionThrown;
            controller.FlightControllerCommandExceptionThrown += Controller_FlightControllerCommandExceptionThrown;
            controller.FlightControllerResponseReceived += Controller_FlightControllerResponseReceived;
            controller.FlightControllerValueReceived += Controller_FlightControllerValueReceived;
            controller.VideoSampleReady += Controller_VideoSampleReady;

            tello.PowerOn();

            controller.EnterSdkMode();

            controller.TakeOff();
            controller.Land();

            WriteLine("press key to end");
            Console.Read();

            tello.PowerOff();
        }

        private static void WriteLine(string text)
        {
            text = $"{DateTime.Now}: {text}";

            Console.WriteLine(text);
            Console.WriteLine("------------------");

            Debug.WriteLine(text);
            Debug.WriteLine("------------------");
        }

        private static void Controller_VideoSampleReady(object sender, VideoSampleReadyArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void Controller_FlightControllerValueReceived(object sender, FlightControllerValueReceivedArgs e)
        {
            WriteLine($"{e.ResponseType} == '{e.Value}' in {e.Elapsed.TotalMilliseconds}ms");
        }

        private static void Controller_FlightControllerResponseReceived(object sender, FlightControllerResponseReceivedArgs e)
        {
            WriteLine($"{e.Command} returned '{e.Response}' in {e.Elapsed.TotalMilliseconds}ms");
        }

        private static void Controller_FlightControllerExceptionThrown(object sender, FlightControllerExceptionThrownArgs e)
        {
            WriteLine($"Exception {e.Exception.GetType()} with message '{e.Exception.Message}'");
            WriteLine("| Stack trace");
            WriteLine($"| {e.Exception.StackTrace}");
        }

        private static void Controller_FlightControllerCommandExceptionThrown(object sender, FlightControllerCommandExceptionThrownArgs e)
        {
            WriteLine($"{e.Command} failed with exception {e.Exception.GetType()} with message '{e.Exception.Message}'");
            WriteLine("| Stack trace");
            WriteLine($"| {e.Exception.StackTrace}");
        }

        private static int _stateCount = 0;
        private static void Controller_DroneStateReceived(object sender, DroneStateReceivedArgs e)
        {
            // state reporting interval is 5hz, so 50 should be once every 10 seconds
            if (_stateCount % 50 == 0)
            {
                var json = JsonConvert.SerializeObject(e.State, Formatting.None);
                WriteLine($"state: {json}");
            }
            _stateCount = _stateCount < Int32.MaxValue
                ? _stateCount + 1
                : 0;
        }
    }
}
