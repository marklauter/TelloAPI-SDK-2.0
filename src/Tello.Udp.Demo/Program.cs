using Messenger.Udp;
using System;
using System.Net;
using System.Threading;
using Tello.Controller;

namespace Tello.Udp.Demo
{
    internal class Program
    {
        private static readonly Drone _tello;

        static Program()
        {
            var transceiver = new UdpTransceiver(IPAddress.Parse("192.168.10.1"), 8889);
            var stateReceiver = new UdpReceiver(8890);
            var videoReceiver = new UdpReceiver(11111);

            _tello = new Drone(transceiver, stateReceiver, videoReceiver);

            _tello.Controller.ConnectionStateChanged += Controller_ConnectionStateChanged;
            _tello.Controller.ExceptionThrown += Controller_ExceptionThrown;
            _tello.Controller.ResponseReceived += Controller_ResponseReceived;

            _tello.StateObserver.StateChanged += StateObserver_StateChanged;
            _tello.VideoObserver.VideoSampleReady += VideoObserver_VideoSampleReady;
        }

        #region events

        private static void Controller_ResponseReceived(object sender, Controller.Events.ResponseReceivedArgs e)
        {
            if(!_canMove 
                && (Command)e.Response.Request.Data == Commands.Takeoff 
                && e.Response.Success && e.Response.Message == Responses.Ok.ToString().ToLowerInvariant())
            {
                _canMove = true;
            }

            Log.WriteLine($"{(Command)e.Response.Request.Data} returned '{e.Response.Message}' in {e.Response.TimeTaken.TotalMilliseconds}ms", ConsoleColor.Cyan);
            Log.WriteLine($"Estimated Position: { _tello.Controller.Position}", ConsoleColor.Blue);
        }

        private static void VideoObserver_VideoSampleReady(object sender, Controller.Events.VideoSampleReadyArgs e)
        {
            throw new NotImplementedException();
        }

        private static int _stateCount = 0;
        private static void StateObserver_StateChanged(object sender, Controller.Events.StateChangedArgs e)
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

        private static void Controller_ExceptionThrown(object sender, Controller.Events.ExceptionThrownArgs e)
        {
            Log.WriteLine($"Exception {e.Exception.InnerException.GetType()} with message '{e.Exception.InnerException.Message}'", ConsoleColor.Red, false);
            Log.WriteLine("| Stack trace", ConsoleColor.Red, false);
            Log.WriteLine($"| {e.Exception.InnerException.StackTrace}", ConsoleColor.Red);
        }

        private static void Controller_ConnectionStateChanged(object sender, Controller.Events.ConnectionStateChangedArgs e)
        {
            Log.WriteLine($"Connection State: {e.Connected}");
            if (e.Connected)
            {
                RunDemo();
            }
        }

        #endregion
        private static bool _canMove = false;
        private static void RunDemo()
        {
            Log.WriteLine("> get battery");
            _tello.Controller.GetBattery();

            Log.WriteLine("> take off");
            _tello.Controller.TakeOff();

            var spinWait = new SpinWait();
            while (!_canMove)
                spinWait.SpinOnce();

            Log.WriteLine("> go forward");
            _tello.Controller.GoForward(50);

            //Log.WriteLine("> turn counter clockwise");
            //_tello.Controller.TurnCounterClockwise(90);

            //Log.WriteLine("> go 50, 50, 50");
            //_tello.Controller.Go(50, 50, -50, 50);

            // sample validation exception
            //try
            //{
            //    Log.WriteLine("> go forward failure example");
            //    _tello.Controller.GoForward(10);
            //}
            //catch (Exception ex)
            //{
            //    Log.WriteLine($"GoForward failed with exception {ex.GetType()} and message '{ex.Message}'", ConsoleColor.Red);
            //}

            //Log.WriteLine("> go forward");
            //_tello.Controller.GoForward(50);

            //Log.WriteLine("> go up");
            //_tello.Controller.GoUp(50);

            //Log.WriteLine("> turn counter clockwise");
            //_tello.Controller.TurnCounterClockwise(90);

            //Log.WriteLine("> turn clockwise");
            //_tello.Controller.TurnClockwise(45);

            Log.WriteLine("> fly polygon");
            _tello.Controller.FlyPolygon(4, 100, 50, ClockDirections.Clockwise, false);

            Log.WriteLine("> land");
            _tello.Controller.Land();

            // you have to do this in real life
            //_tello.PowerOff();
        }

        private static async void Connect()
        {

            // you have to do this in real life
            //_tello.PowerOn();

            Log.WriteLine("> enter sdk mode");
            await _tello.Controller.Connect();

        }

        private static void Main(string[] _)
        {
            Console.WriteLine("Make sure Tello is powered up and you're connected to the network before continuing.");
            Console.WriteLine("press any key when ready to continue...");
            Console.ReadKey(false);

            Connect();

            Console.WriteLine("Remember to turn Tello off to keep it from overheating.");
            Console.WriteLine("press any key when ready to end program...");
            Console.ReadKey(false);
        }
    }
}
