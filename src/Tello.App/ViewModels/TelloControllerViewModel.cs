using Repository;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Tello.App.MvvM;
using Tello.Entities;
using Tello.Entities.Sqlite;
using Tello.State;

namespace Tello.App.ViewModels
{
    public class TelloControllerViewModel : ViewModel
    {
        private readonly IFlightController _controller;
        private readonly IRepository _repository;
        private readonly ISession _session;

        public TelloControllerViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IFlightController controller,
            IRepository repository,
            ISession session)
            : base(dispatcher, notifier)
        {
            _controller = controller ?? throw new ArgumentNullException(nameof(controller));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _controller.ConnectionStateChanged += ConnectionStateChanged;
            _controller.ExceptionThrown += ExceptionThrown;
            _controller.PositionChanged += PositionChanged;
            _controller.ResponseReceived += ResponseReceived;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _controller.ConnectionStateChanged -= ConnectionStateChanged;
            _controller.ExceptionThrown -= ExceptionThrown;
            _controller.PositionChanged -= PositionChanged;
            _controller.ResponseReceived -= ResponseReceived;
        }

        private void ResponseReceived(object sender, Events.ResponseReceivedArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - '{(Command)e.Response.Request.Data}' completed with response '{e.Response.Message}' in {(int)e.Response.TimeTaken.TotalMilliseconds}ms";
            Debug.WriteLine(message);
            Dispatcher.Invoke((msg) =>
            {
                ControlLog.Insert(0, msg as string);
                if (ControlLog.Count > 500)
                {
                    ControlLog.RemoveAt(ControlLog.Count - 1);
                }
            },
            message);

            var group = _repository.NewEntity<ObservationGroup>(_session);
            _repository.Insert(new ResponseObservation(group, e.Response));
        }

        private void PositionChanged(object sender, Events.PositionChangedArgs e)
        {
            Dispatcher.Invoke((position) => 
                Position = position as Vector,
                e.Position);

            var message = $"{DateTime.Now.TimeOfDay} - new position {e.Position}";
            Debug.WriteLine(message);
            Dispatcher.Invoke((msg) =>
            {
                ControlLog.Insert(0, msg as string);
                if (ControlLog.Count > 500)
                {
                    ControlLog.RemoveAt(ControlLog.Count - 1);
                }
            },
            message);
        }

        private void ConnectionStateChanged(object sender, Events.ConnectionStateChangedArgs e)
        {
            Dispatcher.Invoke((connected) =>
                IsConnected = (bool)connected,
                e.Connected);

            var message = $"{DateTime.Now.TimeOfDay} - connected ? {e.Connected}";
            Debug.WriteLine(message);
            Dispatcher.Invoke((msg) =>
            {
                ControlLog.Insert(0, msg as string);
                if (ControlLog.Count > 500)
                {
                    ControlLog.RemoveAt(ControlLog.Count - 1);
                }
            },
            message);
        }

        private void ExceptionThrown(object sender, Events.ExceptionThrownArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - Controller failed with exception '{e.Exception.GetType().FullName}' - {e.Exception.Message} at {e.Exception.StackTrace}";
            Debug.WriteLine(message);
            Dispatcher.Invoke((msg) => ControlLog.Insert(0, msg as string), message);
        }

        public ObservableCollection<string> ControlLog { get; } = new ObservableCollection<string>();

        internal void ClearDatabase()
        {
            if (_repository != null)
            {
                _repository.Delete<ResponseObservation>();
            }
        }

        private bool _isConnected = false;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private Vector _position = new Vector();
        public Vector Position
        {
            get => _position;
            set => SetProperty(ref _position, value);
        }

#pragma warning disable IDE1006 // Naming Styles
        private IInputCommand _ClearDatabaseCommand;
        public IInputCommand ClearDatabaseCommand => _ClearDatabaseCommand = _ClearDatabaseCommand ?? new InputCommand(ClearDatabase);

        private IInputCommand _EnterSdkModeCommand;
        public IInputCommand EnterSdkModeCommand => _EnterSdkModeCommand = _EnterSdkModeCommand ?? new AsyncInputCommand(_controller.Connect);

        private IInputCommand _DisconnectCommand;
        public IInputCommand DisconnectCommand => _DisconnectCommand = _DisconnectCommand ?? new InputCommand(_controller.Disconnect);

        private IInputCommand _TakeOffCommand;
        public IInputCommand TakeOffCommand => _TakeOffCommand = _TakeOffCommand ?? new InputCommand(_controller.TakeOff);

        private IInputCommand _LandCommand;
        public IInputCommand LandCommand => _LandCommand = _LandCommand ?? new InputCommand(_controller.Land);

        private IInputCommand _StopCommand;
        public IInputCommand StopCommand => _StopCommand = _StopCommand ?? new AsyncInputCommand(_controller.Stop);

        private IInputCommand _EmergencyStopCommand;
        public IInputCommand EmergencyStopCommand => _EmergencyStopCommand = _EmergencyStopCommand ?? new AsyncInputCommand(_controller.EmergencyStop);

        private IInputCommand _StartVideoCommand;
        public IInputCommand StartVideoCommand => _StartVideoCommand = _StartVideoCommand ?? new InputCommand(_controller.StartVideo);

        private IInputCommand _StopVideoCommand;
        public IInputCommand StopVideoCommand => _StopVideoCommand = _StopVideoCommand ?? new InputCommand(_controller.StopVideo);

        private IInputCommand<int> _GoUpCommand;
        public IInputCommand<int> GoUpCommand => _GoUpCommand = _GoUpCommand ?? new InputCommand<int>(_controller.GoUp);

        private IInputCommand<int> _GoDownCommand;
        public IInputCommand<int> GoDownCommand => _GoDownCommand = _GoDownCommand ?? new InputCommand<int>(_controller.GoDown);

        private IInputCommand<int> _GoLeftCommand;
        public IInputCommand<int> GoLeftCommand => _GoLeftCommand = _GoLeftCommand ?? new InputCommand<int>(_controller.GoLeft);

        private IInputCommand<int> _GoRightCommand;
        public IInputCommand<int> GoRightCommand => _GoRightCommand = _GoRightCommand ?? new InputCommand<int>(_controller.GoRight);

        private IInputCommand<int> _GoForwardCommand;
        public IInputCommand<int> GoForwardCommand => _GoForwardCommand = _GoForwardCommand ?? new InputCommand<int>(_controller.GoForward);

        private IInputCommand<int> _GoBackwardCommand;
        public IInputCommand<int> GoBackwardCommand => _GoBackwardCommand = _GoBackwardCommand ?? new InputCommand<int>(_controller.GoBackward);

        private IInputCommand<int> _TurnClockwiseCommand;
        public IInputCommand<int> TurnClockwiseCommand => _TurnClockwiseCommand = _TurnClockwiseCommand ?? new InputCommand<int>(_controller.TurnClockwise);

        private IInputCommand<int> _TurnRightCommand;
        public IInputCommand<int> TurnRightCommand => _TurnRightCommand = _TurnRightCommand ?? new InputCommand<int>(_controller.TurnRight);

        private IInputCommand<int> _TurnCounterClockwiseCommand;
        public IInputCommand<int> TurnCounterClockwiseCommand => _TurnCounterClockwiseCommand = _TurnCounterClockwiseCommand ?? new InputCommand<int>(_controller.TurnCounterClockwise);

        private IInputCommand<int> _TurnLeftCommand;
        public IInputCommand<int> TurnLeftCommand => _TurnLeftCommand = _TurnLeftCommand ?? new InputCommand<int>(_controller.TurnLeft);

        private IInputCommand<CardinalDirections> _FlipCommand;
        public IInputCommand<CardinalDirections> FlipCommand => _FlipCommand = _FlipCommand ?? new InputCommand<CardinalDirections>(_controller.Flip);

        private IInputCommand<Tuple<int, int, int, int>> _GoCommand;
        public IInputCommand<Tuple<int, int, int, int>> GoCommand => _GoCommand = _GoCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => _controller.Go(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));

        private IInputCommand<Tuple<int, int, int, int, int, int, int>> _CurveCommand;
        public IInputCommand<Tuple<int, int, int, int, int, int, int>> CommandNameCommand => _CurveCommand = _CurveCommand ?? new InputCommand<Tuple<int, int, int, int, int, int, int>>((tuple) => _controller.Curve(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7));

        //sides, length, speed, clock, do land
        public Tuple<int, int, int, ClockDirections> FlyPolygonCommandParams { get; } = new Tuple<int, int, int, ClockDirections>(3, 100, 50, ClockDirections.Clockwise);
        private IInputCommand<Tuple<int, int, int, ClockDirections>> _FlyPolygonCommand;
        public IInputCommand<Tuple<int, int, int, ClockDirections>> FlyPolygonCommand => 
            _FlyPolygonCommand = 
                _FlyPolygonCommand ?? 
                    new InputCommand<Tuple<int, int, int, ClockDirections>>((tuple) => _controller.FlyPolygon(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));

        private IInputCommand<int> _SetSpeedCommand;
        public IInputCommand<int> SetSpeedCommand => _SetSpeedCommand = _SetSpeedCommand ?? new InputCommand<int>(_controller.SetSpeed);

        private IInputCommand<Tuple<int, int, int, int>> _Set4ChannelRCCommand;
        public IInputCommand<Tuple<int, int, int, int>> Set4ChannelRCCommand => _Set4ChannelRCCommand = _Set4ChannelRCCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => _controller.Set4ChannelRC(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));

        private IInputCommand<Tuple<string, string>> _SetWIFIPasswordCommand;
        public IInputCommand<Tuple<string, string>> SetWIFIPassowrdCommand => _SetWIFIPasswordCommand = _SetWIFIPasswordCommand ?? new InputCommand<Tuple<string, string>>((tuple) => _controller.SetWIFIPassword(tuple.Item1, tuple.Item2));

        private IInputCommand<Tuple<string, string>> _SetStationModeCommand;
        public IInputCommand<Tuple<string, string>> SetStationModeCommand => _SetStationModeCommand = _SetStationModeCommand ?? new InputCommand<Tuple<string, string>>((tuple) => _controller.SetStationMode(tuple.Item1, tuple.Item2));

        private IInputCommand _GetSpeedCommand;
        public IInputCommand GetSpeedCommand => _GetSpeedCommand = _GetSpeedCommand ?? new InputCommand(_controller.GetSpeed);

        private IInputCommand _GetBatteryCommand;
        public IInputCommand GetBatteryCommand => _GetBatteryCommand = _GetBatteryCommand ?? new InputCommand(_controller.GetBattery);

        private IInputCommand _GetTimeCommand;
        public IInputCommand GetTimeCommand => _GetTimeCommand = _GetTimeCommand ?? new InputCommand(_controller.GetTime);

        private IInputCommand _GetWIFISNRCommand;
        public IInputCommand GetWIFISNRCommand => _GetWIFISNRCommand = _GetWIFISNRCommand ?? new InputCommand(_controller.GetWIFISNR);

        private IInputCommand _GetSdkVersionCommand;
        public IInputCommand GetSdkVersionCommand => _GetSdkVersionCommand = _GetSdkVersionCommand ?? new InputCommand(_controller.GetSdkVersion);

        private IInputCommand _GetSerialNumberCommand;
        public IInputCommand GetSerialNumberCommand => _GetSerialNumberCommand = _GetSerialNumberCommand ?? new InputCommand(_controller.GetSerialNumber);
#pragma warning restore IDE1006 // Naming Styles

    }
}
