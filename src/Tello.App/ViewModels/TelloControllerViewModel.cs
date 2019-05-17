using Repository;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Entities;

namespace Tello.App.ViewModels
{
    public class TelloControllerViewModel : ViewModel
    {
        private readonly DroneMessenger _tello;
        private readonly IRepository _repository;
        private readonly ISession _session;

        public TelloControllerViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            DroneMessenger tello,
            IRepository repository,
            ISession session)
            : base(dispatcher, notifier)
        {
            _tello = tello ?? throw new ArgumentNullException(nameof(tello));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _tello.ExceptionThrown += OnExceptionThrown;
            _tello.CommandExceptionThrown += OnCommandExceptionThrown;
            _tello.QueryResponseReceived += OnQueryResponseReceived;
            _tello.CommandResponseReceived += OnCommandResponseReceived;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _tello.ExceptionThrown -= OnExceptionThrown;
            _tello.CommandExceptionThrown -= OnCommandExceptionThrown;
            _tello.QueryResponseReceived -= OnQueryResponseReceived;
            _tello.CommandResponseReceived -= OnCommandResponseReceived;
        }

        private void OnCommandExceptionThrown(object sender, CommandExceptionThrownArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - Command {e.Command} failed with exception '{e.Exception.GetType().FullName}' - {e.Exception.Message}";
            Debug.WriteLine(message);
            Dispatcher.Invoke((msg) => { ControlLog.Insert(0, msg as string); }, message);
        }

        private void OnExceptionThrown(object sender, ExceptionThrownArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - Controller failed with exception '{e.Exception.GetType().FullName}' - {e.Exception.Message} at {e.Exception.StackTrace}";
            Debug.WriteLine(message);
            Dispatcher.Invoke((msg) => { ControlLog.Insert(0, msg as string); }, message);
        }

        private void OnCommandResponseReceived(object sender, CommandResponseReceivedArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - {e.Command} completed with response '{e.Response}' in {Convert.ToInt32(e.Elapsed.TotalMilliseconds)}ms";
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

            if (_repository != null)
            {
                _repository.Write(new TelloCommandObservation(e, null));
            }
        }

        private void OnQueryResponseReceived(object sender, QueryResponseReceivedArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - {e.ResponseType} returned value '{e.Value}' int {Convert.ToInt32(e.Elapsed.TotalMilliseconds)}ms";
            Debug.WriteLine(message);
            Dispatcher.Invoke((msg) => { ControlLog.Insert(0, msg as string); }, message);
        }

        public ObservableCollection<string> ControlLog { get; } = new ObservableCollection<string>();

        internal void ClearDatabase()
        {
            if (_repository != null)
            {
                _repository.Delete<TelloCommandObservation>();
            }
        }

#pragma warning disable IDE1006 // Naming Styles
        private IInputCommand _ClearDatabaseCommand;
        public IInputCommand ClearDatabaseCommand => _ClearDatabaseCommand = _ClearDatabaseCommand ?? new InputCommand(ClearDatabase);

        private IInputCommand _EnterSdkModeCommand;
        public IInputCommand EnterSdkModeCommand => _EnterSdkModeCommand = _EnterSdkModeCommand ?? new InputCommand(_tello.Connect);

        private IInputCommand _DisconnectCommand;
        public IInputCommand DisconnectCommand => _DisconnectCommand = _DisconnectCommand ?? new InputCommand(_tello.Disconnect);

        private IInputCommand _TakeOffCommand;
        public IInputCommand TakeOffCommand => _TakeOffCommand = _TakeOffCommand ?? new InputCommand(_tello.TakeOff);

        private IInputCommand _LandCommand;
        public IInputCommand LandCommand => _LandCommand = _LandCommand ?? new InputCommand(_tello.Land);

        private IInputCommand _StopCommand;
        public IInputCommand StopCommand => _StopCommand = _StopCommand ?? new InputCommand(_tello.Stop);

        private IInputCommand _EmergencyStopCommand;
        public IInputCommand EmergencyStopCommand => _EmergencyStopCommand = _EmergencyStopCommand ?? new InputCommand(_tello.EmergencyStop);

        private IInputCommand _StartVideoCommand;
        public IInputCommand StartVideoCommand => _StartVideoCommand = _StartVideoCommand ?? new InputCommand(_tello.StartVideo);

        private IInputCommand _StopVideoCommand;
        public IInputCommand StopVideoCommand => _StopVideoCommand = _StopVideoCommand ?? new InputCommand(_tello.StopVideo);

        private IInputCommand<int> _GoUpCommand;
        public IInputCommand<int> GoUpCommand => _GoUpCommand = _GoUpCommand ?? new InputCommand<int>(_tello.GoUp);

        private IInputCommand<int> _GoDownCommand;
        public IInputCommand<int> GoDownCommand => _GoDownCommand = _GoDownCommand ?? new InputCommand<int>(_tello.GoDown);

        private IInputCommand<int> _GoLeftCommand;
        public IInputCommand<int> GoLeftCommand => _GoLeftCommand = _GoLeftCommand ?? new InputCommand<int>(_tello.GoLeft);

        private IInputCommand<int> _GoRightCommand;
        public IInputCommand<int> GoRightCommand => _GoRightCommand = _GoRightCommand ?? new InputCommand<int>(_tello.GoRight);

        private IInputCommand<int> _GoForwardCommand;
        public IInputCommand<int> GoForwardCommand => _GoForwardCommand = _GoForwardCommand ?? new InputCommand<int>(_tello.GoForward);

        private IInputCommand<int> _GoBackwardCommand;
        public IInputCommand<int> GoBackwardCommand => _GoBackwardCommand = _GoBackwardCommand ?? new InputCommand<int>(_tello.GoBackward);

        private IInputCommand<int> _TurnClockwiseCommand;
        public IInputCommand<int> TurnClockwiseCommand => _TurnClockwiseCommand = _TurnClockwiseCommand ?? new InputCommand<int>(_tello.TurnClockwise);

        private IInputCommand<int> _TurnRightCommand;
        public IInputCommand<int> TurnRightCommand => _TurnRightCommand = _TurnRightCommand ?? new InputCommand<int>(_tello.TurnRight);

        private IInputCommand<int> _TurnCounterClockwiseCommand;
        public IInputCommand<int> TurnCounterClockwiseCommand => _TurnCounterClockwiseCommand = _TurnCounterClockwiseCommand ?? new InputCommand<int>(_tello.TurnCounterClockwise);

        private IInputCommand<int> _TurnLeftCommand;
        public IInputCommand<int> TurnLeftCommand => _TurnLeftCommand = _TurnLeftCommand ?? new InputCommand<int>(_tello.TurnLeft);

        private IInputCommand<CardinalDirections> _FlipCommand;
        public IInputCommand<CardinalDirections> FlipCommand => _FlipCommand = _FlipCommand ?? new InputCommand<CardinalDirections>(_tello.Flip);

        private IInputCommand<Tuple<int, int, int, int>> _GoCommand;
        public IInputCommand<Tuple<int, int, int, int>> GoCommand => _GoCommand = _GoCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => { _tello.Go(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4); });

        private IInputCommand<Tuple<int, int, int, int, int, int, int>> _CurveCommand;
        public IInputCommand<Tuple<int, int, int, int, int, int, int>> CommandNameCommand => _CurveCommand = _CurveCommand ?? new InputCommand<Tuple<int, int, int, int, int, int, int>>((tuple) => { _tello.Curve(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7); });

        //sides, length, speed, clock, do land
        public Tuple<int, int, int, ClockDirections, bool> FlyPolygonCommandParams { get; } = new Tuple<int, int, int, ClockDirections, bool>(3, 100, 50, ClockDirections.Clockwise, false);
        private IInputCommand<Tuple<int, int, int, ClockDirections, bool>> _FlyPolygonCommand;
        public IInputCommand<Tuple<int, int, int, ClockDirections, bool>> FlyPolygonCommand => _FlyPolygonCommand = _FlyPolygonCommand ?? new InputCommand<Tuple<int, int, int, ClockDirections, bool>>((tuple) => { _tello.FlyPolygon(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5); });

        private IInputCommand<int> _SetSpeedCommand;
        public IInputCommand<int> SetSpeedCommand => _SetSpeedCommand = _SetSpeedCommand ?? new InputCommand<int>(_tello.SetSpeed);

        private IInputCommand<Tuple<int, int, int, int>> _Set4ChannelRCCommand;
        public IInputCommand<Tuple<int, int, int, int>> Set4ChannelRCCommand => _Set4ChannelRCCommand = _Set4ChannelRCCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => { _tello.Set4ChannelRC(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4); });

        private IInputCommand<Tuple<string, string>> _SetWIFIPasswordCommand;
        public IInputCommand<Tuple<string, string>> SetWIFIPassowrdCommand => _SetWIFIPasswordCommand = _SetWIFIPasswordCommand ?? new InputCommand<Tuple<string, string>>((tuple) => { _tello.SetWIFIPassword(tuple.Item1, tuple.Item2); });

        private IInputCommand<Tuple<string, string>> _SetStationModeCommand;
        public IInputCommand<Tuple<string, string>> SetStationModeCommand => _SetStationModeCommand = _SetStationModeCommand ?? new InputCommand<Tuple<string, string>>((tuple) => { _tello.SetStationMode(tuple.Item1, tuple.Item2); });

        private IInputCommand _GetSpeedCommand;
        public IInputCommand GetSpeedCommand => _GetSpeedCommand = _GetSpeedCommand ?? new InputCommand(_tello.GetSpeed);

        private IInputCommand _GetBatteryCommand;
        public IInputCommand GetBatteryCommand => _GetBatteryCommand = _GetBatteryCommand ?? new InputCommand(_tello.GetBattery);

        private IInputCommand _GetTimeCommand;
        public IInputCommand GetTimeCommand => _GetTimeCommand = _GetTimeCommand ?? new InputCommand(_tello.GetTime);

        private IInputCommand _GetWIFISNRCommand;
        public IInputCommand GetWIFISNRCommand => _GetWIFISNRCommand = _GetWIFISNRCommand ?? new InputCommand(_tello.GetWIFISNR);

        private IInputCommand _GetSdkVersionCommand;
        public IInputCommand GetSdkVersionCommand => _GetSdkVersionCommand = _GetSdkVersionCommand ?? new InputCommand(_tello.GetSdkVersion);

        private IInputCommand _GetWIFISerialNumberCommand;
        public IInputCommand GetWiFiSerialNumberCommand => _GetWIFISerialNumberCommand = _GetWIFISerialNumberCommand ?? new InputCommand(_tello.GetWIFISerialNumber);
#pragma warning restore IDE1006 // Naming Styles

    }
}
