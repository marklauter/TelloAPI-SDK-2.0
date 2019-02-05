using System;
using System.Text;
using Tello.Controller;

namespace Tello.Udp.Demo
{
    internal class Program
    {
        static Program()
        {
            _tello = new UdpMessenger();
            _controller = new FlightController(_tello, _tello.StateServer, _tello.VideoServer);
        }

        private static readonly UdpMessenger _tello;
        private static readonly FlightController _controller;

        private static void Main(string[] args)
        {
            _controller.DroneStateReceived += Controller_DroneStateReceived;
            _controller.FlightControllerExceptionThrown += Controller_FlightControllerExceptionThrown;
            _controller.FlightControllerCommandExceptionThrown += Controller_FlightControllerCommandExceptionThrown;
            _controller.FlightControllerResponseReceived += Controller_FlightControllerResponseReceived;
            _controller.FlightControllerValueReceived += Controller_FlightControllerValueReceived;
            _controller.VideoSampleReady += Controller_VideoSampleReady;

            Console.WriteLine("Make sure Tello is powered up and you're connected to the network before continuing.");
            Console.WriteLine("press any key when ready to continue...");
            Console.ReadKey(false);

            // you have to do this in real life
            //_tello.PowerOn();

            Log.WriteLine("> enter sdk mode");

            _controller.EnterSdkMode();

            Log.WriteLine("> get battery");
            _controller.GetBattery();

            Log.WriteLine("> take off");
            _controller.TakeOff();
            try
            {
                Log.WriteLine("> go forward failure example");
                _controller.GoForward(10);
            }
            catch (Exception ex)
            {
                Log.WriteLine($"GoForward failed with exception {ex.GetType()} and message '{ex.Message}'", ConsoleColor.Red);
            }

            Log.WriteLine("> go forward");
            _controller.GoForward(50);

            Log.WriteLine("> go up");
            _controller.GoUp(50);

            Log.WriteLine("> turn clockwise");
            _controller.TurnCounterClockwise(90);

            Log.WriteLine("> turn counter clockwise");
            _controller.TurnClockwise(45);

            Log.WriteLine("> fly polygon");
            _controller.FlyPolygon(4, 100, 50, FlightController.ClockDirections.Clockwise, false);


            Log.WriteLine("> land");
            _controller.Land();

            Console.WriteLine("Remember to turn Tello off to keep it from overheating.");
            Console.WriteLine("press any key when ready to continue...");
            Console.ReadKey(false);

            // you have to do this in real life
            //_tello.PowerOff();
        }

        private static void Controller_VideoSampleReady(object sender, VideoSampleReadyArgs e)
        {
            //throw new NotImplementedException();
        }

        private static void Controller_FlightControllerValueReceived(object sender, FlightControllerValueReceivedArgs e)
        {
            Log.WriteLine($"{e.ResponseType} == '{e.Value}' in {e.Elapsed.TotalMilliseconds}ms", ConsoleColor.Green);
        }

        private static void Controller_FlightControllerResponseReceived(object sender, FlightControllerResponseReceivedArgs e)
        {
            Log.WriteLine($"{e.Command} returned '{e.Response}' in {e.Elapsed.TotalMilliseconds}ms", ConsoleColor.Cyan);
            //todo: would be nice to add position to the flight controller so we could keep track of the expected position (disregarding drone drift)
            //Log.WriteLine($"Position: { _tello.Position}");
        }

        private static void Controller_FlightControllerExceptionThrown(object sender, FlightControllerExceptionThrownArgs e)
        {
            Log.WriteLine($"Exception {e.Exception.GetType()} with message '{e.Exception.Message}'", ConsoleColor.Red, false);
            Log.WriteLine("| Stack trace", ConsoleColor.Red, false);
            Log.WriteLine($"| {e.Exception.StackTrace}", ConsoleColor.Red);
        }

        private static void Controller_FlightControllerCommandExceptionThrown(object sender, FlightControllerCommandExceptionThrownArgs e)
        {
            Log.WriteLine($"{e.Command} failed with exception {e.Exception.GetType()} with message '{e.Exception.Message}' after {e.Elapsed.TotalMilliseconds}ms", ConsoleColor.Red, false);
            Log.WriteLine("| Stack trace", ConsoleColor.Red, false);
            Log.WriteLine($"| {e.Exception.StackTrace}", ConsoleColor.Red);
        }

        private static int _stateCount = 0;
        private static void Controller_DroneStateReceived(object sender, DroneStateReceivedArgs e)
        {
            // state reporting interval is 5hz, so 25 should be once every 5 seconds
            if (_stateCount % 25 == 0)
            {
                Log.WriteLine($"state: {e.State}", ConsoleColor.Yellow);
            }
            _stateCount = _stateCount < Int32.MaxValue
                ? _stateCount + 1
                : 0;
        }
    }
}
