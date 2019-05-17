using Messenger.Udp;
using Repository;
using Repository.Sqlite;
using System;
using System.Net;
using System.Threading;
using Tello.Controller;
using Tello.Entities;
using Tello.Entities.Sqlite;

namespace Tello.Udp.Demo
{
    internal class Program
    {
        private static readonly DroneMessenger _tello;
        private static readonly IRepository _repository;
        private static readonly ISession _session;

        static Program()
        {
            var transceiver = new UdpTransceiver(IPAddress.Parse("192.168.10.1"), 8889);
            var stateReceiver = new UdpReceiver(8890);
            var videoReceiver = new UdpReceiver(11111);

            _tello = new DroneMessenger(transceiver, stateReceiver, videoReceiver);

            _tello.Controller.ConnectionStateChanged += Controller_ConnectionStateChanged;
            _tello.Controller.ExceptionThrown += Controller_ExceptionThrown;
            _tello.Controller.ResponseReceived += Controller_ResponseReceived;

            _tello.StateObserver.StateChanged += StateObserver_StateChanged;
            _tello.VideoObserver.VideoSampleReady += VideoObserver_VideoSampleReady;

            _repository = new SqliteRepository((null, "tello.udp.sqlite"));
            _repository.CreateCatalog<Session>();
            _repository.CreateCatalog<ObservationGroup>();
            _repository.CreateCatalog<StateObservation>();
            _repository.CreateCatalog<AirSpeedObservation>();
            _repository.CreateCatalog<AttitudeObservation>();
            _repository.CreateCatalog<BatteryObservation>();
            _repository.CreateCatalog<HobbsMeterObservation>();
            _repository.CreateCatalog<PositionObservation>();
            _repository.CreateCatalog<ResponseObservation>();

            _session = _repository.NewEntity<Session>();
        }

        #region event handlers

        private static void Controller_ResponseReceived(object sender, Events.ResponseReceivedArgs e)
        {
            if (!_canMove
                && (Command)e.Response.Request.Data == Commands.Takeoff
                && e.Response.Success && e.Response.Message == Responses.Ok.ToString().ToLowerInvariant())
            {
                _canMove = true;
            }

            Log.WriteLine($"'{(Command)e.Response.Request.Data}' returned '{e.Response.Message}' in {(int)e.Response.TimeTaken.TotalMilliseconds}ms", ConsoleColor.Cyan);
            Log.WriteLine($"Estimated Position: { _tello.Controller.Position}", ConsoleColor.Blue);

            var group = _repository.NewEntity<ObservationGroup>(_session);
            _repository.Insert(new ResponseObservation(group, e.Response));
        }

        private static void VideoObserver_VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            throw new NotImplementedException();
        }

        private static int _stateCount = 0;
        private static void StateObserver_StateChanged(object sender, Events.StateChangedArgs e)
        {
            // state reporting interval is 5hz, so 25 should be once every 5 seconds
            if (_stateCount % 25 == 0)
            {
                Log.WriteLine($"state: {e.State}", ConsoleColor.Yellow);
            }
            _stateCount = _stateCount < Int32.MaxValue
                ? _stateCount + 1
                : 0;

            var group = _repository.NewEntity<ObservationGroup>(_session);
            _repository.Insert(new StateObservation(group, e.State));
            _repository.Insert(new AirSpeedObservation(group, e.State));
            _repository.Insert(new AttitudeObservation(group, e.State));
            _repository.Insert(new BatteryObservation(group, e.State));
            _repository.Insert(new HobbsMeterObservation(group, e.State));
            _repository.Insert(new PositionObservation(group, e.State));
        }

        private static void Controller_ExceptionThrown(object sender, Events.ExceptionThrownArgs e)
        {
            Log.WriteLine($"Exception {e.Exception.InnerException.GetType()} with message '{e.Exception.InnerException.Message}'", ConsoleColor.Red, false);
            Log.WriteLine("| Stack trace", ConsoleColor.Red, false);
            Log.WriteLine($"| {e.Exception.InnerException.StackTrace}", ConsoleColor.Red);
        }

        private static void Controller_ConnectionStateChanged(object sender, Events.ConnectionStateChangedArgs e)
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
            {
                spinWait.SpinOnce();
            }

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
            _tello.Controller.FlyPolygon(4, 100, 50, ClockDirections.Clockwise);

            Log.WriteLine("> land");
            _tello.Controller.Land();
        }

        private static async void Connect()
        {
            Log.WriteLine("> enter sdk mode");
            if (!await _tello.Controller.Connect())
            {
                Log.WriteLine("connection failed - program will be terminated");
                Console.ReadKey(false);
            }
            else
            {
                Console.WriteLine("Remember to turn Tello off to keep it from overheating.");
                Console.WriteLine("press any key when ready to end program...");
                Console.ReadKey(false);
            }
            _repository.Dispose();
        }

        private static void Main(string[] _)
        {
            Console.WriteLine("Make sure Tello is powered up and you're connected to the network before continuing.");
            Console.WriteLine("press any key when ready to continue...");
            Console.ReadKey(false);

            Connect();
        }
    }
}
