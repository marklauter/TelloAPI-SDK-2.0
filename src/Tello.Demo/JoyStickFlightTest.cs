using Messenger;
using Repository;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Tello.Controller;
using Tello.Entities.Sqlite;


namespace Tello.Demo
{
    public sealed class JoyStickFlightTest : IFlightTest
    {
        private readonly DroneMessenger _tello;
        private readonly IRepository _repository;
        private readonly Session _session;
        private bool _canMove = false;

        public JoyStickFlightTest(
            IRepository repository,
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
        {
            _tello = new DroneMessenger(transceiver, stateReceiver, videoReceiver);
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _tello.Controller.ConnectionStateChanged += Controller_ConnectionStateChanged;
            _tello.Controller.ExceptionThrown += Controller_ExceptionThrown;
            _tello.Controller.ResponseReceived += Controller_ResponseReceived;

            _tello.StateObserver.StateChanged += StateObserver_StateChanged;
            _tello.VideoObserver.VideoSampleReady += VideoObserver_VideoSampleReady;

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

        public async Task Invoke()
        {
            Console.WriteLine("Make sure Tello is powered up and you're connected to the network before continuing.");
            Console.WriteLine("press any key when ready to start flight test...");
            Console.ReadKey(true);

            Log.WriteLine("> enter sdk mode");
            if (await _tello.Controller.Connect())
            {
                Console.WriteLine("Remember to turn Tello off to keep it from overheating.");
                Console.WriteLine("press any key when ready to end program...");
            }
            else
            {
                Log.WriteLine("connection failed - program will be terminated");
            }

            Console.ReadKey(true);

            if (_videoFile != null)
            {
                _videoFile.Close();
            }

            _repository.Dispose();
        }

        private void Continue()
        {
            //Log.WriteLine("> start video");
            //_tello.Controller.StartVideo();

            Log.WriteLine("> take off");
            _tello.Controller.TakeOff();

            var spinWait = new SpinWait();
            while (!_canMove)
            {
                spinWait.SpinOnce();
            }

            Console.WriteLine("'w' to go forward");
            Console.WriteLine("'s' to go backward");
            Console.WriteLine("'a' to go left");
            Console.WriteLine("'d' to go right");
            Console.WriteLine("up arrow to go up");
            Console.WriteLine("down arrow to go down");
            Console.WriteLine("right arrow to turn right");
            Console.WriteLine("left arrow to turn left");
            Console.WriteLine("'e' to exit");

            var e = Console.ReadKey(true);
            while (e.Key != ConsoleKey.E)
            {
                switch (e.Key)
                {
                    case ConsoleKey.W: // forward
                        _tello.Controller.Set4ChannelRC(0, 50, 0, 0);
                        Console.Write("F");
                        break;
                    case ConsoleKey.S: // backward
                        _tello.Controller.Set4ChannelRC(0, -50, 0, 0);
                        Console.Write("B");
                        break;
                    case ConsoleKey.D: // right
                        _tello.Controller.Set4ChannelRC(50, 0, 0, 0);
                        Console.Write("R");
                        break;
                    case ConsoleKey.A: // left
                        _tello.Controller.Set4ChannelRC(-50, 0, 0, 0);
                        Console.Write("L");
                        break;
                    case ConsoleKey.UpArrow: // up
                        _tello.Controller.Set4ChannelRC(0, 0, 50, 0);
                        Console.Write("U");
                        break;
                    case ConsoleKey.DownArrow: // down
                        _tello.Controller.Set4ChannelRC(0, 0, -50, 0);
                        Console.Write("D");
                        break;
                    case ConsoleKey.LeftArrow: // turn left
                        _tello.Controller.Set4ChannelRC(0, 0, 0, -50);
                        Console.Write("YL");
                        break;
                    case ConsoleKey.RightArrow: // right
                        _tello.Controller.Set4ChannelRC(0, 0, 0, 50);
                        Console.Write("YR");
                        break;
                    case ConsoleKey.E: // exit
                        Console.WriteLine();
                        Console.WriteLine("EXIT");
                        break;
                    default:
                        _tello.Controller.Set4ChannelRC(0, 0, 0, 0);
                        break;
                }
                spinWait.SpinOnce();
                e = Console.ReadKey(true);
            }

            Log.WriteLine("> land");
            _tello.Controller.Land();
        }


        #region event handlers
        private void Controller_ResponseReceived(object sender, Events.ResponseReceivedArgs e)
        {
            if (!_canMove
                && (Command)e.Response.Request.Data == Commands.Takeoff
                && e.Response.Success && e.Response.Message == Responses.Ok.ToString().ToLowerInvariant())
            {
                _canMove = true;
            }

            Log.WriteLine($"{(Command)e.Response.Request.Data} returned '{e.Response.Message}' in {(int)e.Response.TimeTaken.TotalMilliseconds}ms", ConsoleColor.Cyan);

            var group = _repository.NewEntity<ObservationGroup>(_session);
            _repository.Insert(new ResponseObservation(group, e.Response));
        }

        private long _videoLength = 0;
        private FileStream _videoFile = null;

        private void VideoObserver_VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            _videoFile = _videoFile ?? File.OpenWrite($"tello.video.{_session.Id}.h264");
            _videoFile.Write(e.Message.Data, 0, e.Message.Data.Length);

            _videoLength = _videoLength < Int64.MaxValue
                ? _videoLength + e.Message.Data.Length
                : 0;
        }

        private void StateObserver_StateChanged(object sender, Events.StateChangedArgs e)
        {
            var group = _repository.NewEntity<ObservationGroup>(_session);
            _repository.Insert(new StateObservation(group, e.State));
            _repository.Insert(new AirSpeedObservation(group, e.State));
            _repository.Insert(new AttitudeObservation(group, e.State));
            _repository.Insert(new BatteryObservation(group, e.State));
            _repository.Insert(new HobbsMeterObservation(group, e.State));
            _repository.Insert(new PositionObservation(group, e.State));
        }

        private void Controller_ExceptionThrown(object sender, Events.ExceptionThrownArgs e)
        {
            Log.WriteLine($"Exception {e.Exception.InnerException.GetType()} with message - {e.Exception.InnerException.Message}", ConsoleColor.Red, false);
            Log.WriteLine("| Stack trace", ConsoleColor.Red, false);
            Log.WriteLine($"| {e.Exception.InnerException.StackTrace}", ConsoleColor.Red);
        }

        private void Controller_ConnectionStateChanged(object sender, Events.ConnectionStateChangedArgs e)
        {
            Log.WriteLine($"Connection State: {e.IsConnected}");
            if (e.IsConnected)
            {
                Continue();
            }
        }
        #endregion
    }
}
