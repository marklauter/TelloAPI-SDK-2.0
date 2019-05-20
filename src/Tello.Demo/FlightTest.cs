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
    public sealed class FlightTest
    {
        private readonly DroneMessenger _tello;
        private readonly IRepository _repository;
        private readonly Session _session;
        private bool _canMove = false;

        public FlightTest(
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
            Console.ReadKey(false);

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
            Console.ReadKey(false);

            if (_videoFile != null)
                _videoFile.Close();

            _repository.Dispose();
        }

        private void Continue()
        {
            Log.WriteLine("> get battery");
            _tello.Controller.GetBattery();

            Log.WriteLine("> start video");
            _tello.Controller.StartVideo();

            Log.WriteLine("> take off");
            _tello.Controller.TakeOff();

            var spinWait = new SpinWait();
            while (!_canMove)
            {
                spinWait.SpinOnce();
            }

            Log.WriteLine("> go forward");
            _tello.Controller.GoForward(50);

            Log.WriteLine("> go backward");
            _tello.Controller.GoBackward(50);

            //Log.WriteLine("> fly polygon");
            //_tello.Controller.FlyPolygon(4, 100, 50, ClockDirections.Clockwise);

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
            Log.WriteLine($"Estimated Position: { _tello.Controller.Position}", ConsoleColor.Blue);

            var group = _repository.NewEntity<ObservationGroup>(_session);
            _repository.Insert(new ResponseObservation(group, e.Response));
        }

        private int _videoCount = 0;
        private long _videoLength = 0;
        private FileStream _videoFile = null;

        private void VideoObserver_VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            _videoFile = _videoFile == null
                ? File.OpenWrite($"tello.video.{_session.Id}.h264")
                : _videoFile;
            _videoFile.Write(e.Message.Data, 0, e.Message.Data.Length);

            _videoLength = _videoLength < Int64.MaxValue
                ? _videoLength + e.Message.Data.Length
                : 0;

            // video reporting interval is 30hz, so 150 should be once every 5 seconds
            if (_videoCount % 150 == 0)
            {
                Log.WriteLine($"video segment size {e.Message.Data.Length}b @ {e.Message.Timestamp.ToString("o")}", ConsoleColor.Magenta, false);
                Log.WriteLine($"ttl video size {_videoLength}b, segment count {_videoCount + 1}", ConsoleColor.Magenta);
            }


            _videoCount = _videoCount < Int32.MaxValue
                ? _videoCount + 1
                : 0;
        }

        private int _stateCount = 0;
        private void StateObserver_StateChanged(object sender, Events.StateChangedArgs e)
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
