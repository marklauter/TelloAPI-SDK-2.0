using System;
using System.Collections.ObjectModel;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.App.ViewModels
{
    public class TelloControllerViewModel : ViewModel
    {
        private readonly ITelloController _telloController;

        public TelloControllerViewModel(IUIDispatcher dispatcher, IUserNotifier userNotifier, ITelloController telloController) : base(dispatcher, userNotifier)
        {
            _telloController = telloController ?? throw new ArgumentNullException(nameof(telloController));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _telloController.ExceptionThrown += OnExceptionThrown;
            _telloController.CommandExceptionThrown += OnCommandExceptionThrown;
            _telloController.QueryResponseReceived += OnQueryResponseReceived;
            _telloController.CommandResponseReceived += OnCommandResponseReceived;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _telloController.ExceptionThrown -= OnExceptionThrown;
            _telloController.CommandExceptionThrown -= OnCommandExceptionThrown;
            _telloController.QueryResponseReceived -= OnQueryResponseReceived;
            _telloController.CommandResponseReceived -= OnCommandResponseReceived;
        }

        private void OnCommandExceptionThrown(object sender, CommandExceptionThrownArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - Command {e.Command} failed with exception {e.Exception.GetType().FullName} - {e.Exception.Message}";
            ControlLog.Add(message);
        }

        private void OnExceptionThrown(object sender, ExceptionThrownArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - Controller failed with exception {e.Exception.GetType().FullName} - {e.Exception.Message} at {e.Exception.StackTrace}";
            ControlLog.Add(message);
        }

        private void OnCommandResponseReceived(object sender, CommandResponseReceivedArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - {e.Command} completed with response {e.Response} in {Convert.ToInt32(e.Elapsed.TotalMilliseconds)}ms";
            ControlLog.Add(message);
        }

        private void OnQueryResponseReceived(object sender, QueryResponseReceivedArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - {e.ResponseType} received with value '{e.Value}' int {Convert.ToInt32(e.Elapsed.TotalMilliseconds)}ms";
            ControlLog.Add(message);
        }

        public ObservableCollection<string> ControlLog = new ObservableCollection<string>();

#pragma warning disable IDE1006 // Naming Styles
        private IInputCommand _EnterSdkModeCommand;
        public IInputCommand EnterSdkModeCommand => _EnterSdkModeCommand = _EnterSdkModeCommand ?? new InputCommand(_telloController.EnterSdkMode);

        private IInputCommand _TakeOffCommand;
        public IInputCommand TakeOffCommand => _TakeOffCommand = _TakeOffCommand ?? new InputCommand(_telloController.TakeOff);

        private IInputCommand _LandCommand;
        public IInputCommand LandCommand => _LandCommand = _LandCommand ?? new InputCommand(_telloController.Land);

        private IInputCommand _StopCommand;
        public IInputCommand StopCommand => _StopCommand = _StopCommand ?? new InputCommand(_telloController.Stop);

        private IInputCommand _EmergencyStopCommand;
        public IInputCommand EmergencyStopCommand => _EmergencyStopCommand = _EmergencyStopCommand ?? new InputCommand(_telloController.EmergencyStop);

        private IInputCommand _StartVideoCommand;
        public IInputCommand StartVideoCommand => _StartVideoCommand = _StartVideoCommand ?? new InputCommand(_telloController.StartVideo);

        private IInputCommand _StopVideoCommand;
        public IInputCommand StopVideoCommand => _StopVideoCommand = _StopVideoCommand ?? new InputCommand(_telloController.StopVideo);

        private IInputCommand<int> _GoUpCommand;
        public IInputCommand<int> GoUpCommand => _GoUpCommand = _GoUpCommand ?? new InputCommand<int>(_telloController.GoUp);

        private IInputCommand<int> _GoDownCommand;
        public IInputCommand<int> GoDownCommand => _GoDownCommand = _GoDownCommand ?? new InputCommand<int>(_telloController.GoDown);

        private IInputCommand<int> _GoLeftCommand;
        public IInputCommand<int> GoLeftCommand => _GoLeftCommand = _GoLeftCommand ?? new InputCommand<int>(_telloController.GoLeft);

        private IInputCommand<int> _GoRightCommand;
        public IInputCommand<int> GoRightCommand => _GoRightCommand = _GoRightCommand ?? new InputCommand<int>(_telloController.GoRight);

        private IInputCommand<int> _GoBackwardCommand;
        public IInputCommand<int> GoBackwardCommand => _GoBackwardCommand = _GoBackwardCommand ?? new InputCommand<int>(_telloController.GoBackward);

        private IInputCommand<int> _TurnClockwiseCommand;
        public IInputCommand<int> TurnClockwiseCommand => _TurnClockwiseCommand = _TurnClockwiseCommand ?? new InputCommand<int>(_telloController.TurnClockwise);

        private IInputCommand<int> _TurnRightCommand;
        public IInputCommand<int> TurnRightCommand => _TurnRightCommand = _TurnRightCommand ?? new InputCommand<int>(_telloController.TurnRight);

        private IInputCommand<int> _TurnCounterClockwiseCommand;
        public IInputCommand<int> TurnCounterClockwiseCommand => _TurnCounterClockwiseCommand = _TurnCounterClockwiseCommand ?? new InputCommand<int>(_telloController.TurnCounterClockwise);

        private IInputCommand<int> _TurnLeftCommand;
        public IInputCommand<int> TurnLeftCommand => _TurnLeftCommand = _TurnLeftCommand ?? new InputCommand<int>(_telloController.TurnLeft);

        private IInputCommand<CardinalDirections> _FlipCommand;
        public IInputCommand<CardinalDirections> FlipCommand => _FlipCommand = _FlipCommand ?? new InputCommand<CardinalDirections>(_telloController.Flip);

        private IInputCommand<Tuple<int, int, int, int>> _GoCommand;
        public IInputCommand<Tuple<int, int, int, int>> GoCommand => _GoCommand = _GoCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => { _telloController.Go(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4); });

        private IInputCommand<Tuple<int, int, int, int, int, int, int>> _CurveCommand;
        public IInputCommand<Tuple<int, int, int, int, int, int, int>> CommandNameCommand => _CurveCommand = _CurveCommand ?? new InputCommand<Tuple<int, int, int, int, int, int, int>>((tuple) => { _telloController.Curve(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7); });

        private IInputCommand<Tuple<int, int, int, ClockDirections, bool>> _FlyPolygonCommand;
        public IInputCommand<Tuple<int, int, int, ClockDirections, bool>> FlyPolygonCommand => _FlyPolygonCommand = _FlyPolygonCommand ?? new InputCommand<Tuple<int, int, int, ClockDirections, bool>>((tuple) => { _telloController.FlyPolygon(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5); });

        private IInputCommand<int> _SetSpeedCommand;
        public IInputCommand<int> SetSpeedCommand => _SetSpeedCommand = _SetSpeedCommand ?? new InputCommand<int>(_telloController.SetSpeed);

        private IInputCommand<Tuple<int, int, int, int>> _Set4ChannelRCCommand;
        public IInputCommand<Tuple<int, int, int, int>> Set4ChannelRCCommand => _Set4ChannelRCCommand = _Set4ChannelRCCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => { _telloController.Set4ChannelRC(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4); });

        private IInputCommand<Tuple<string, string>> _SetWIFIPasswordCommand;
        public IInputCommand<Tuple<string, string>> SetWIFIPassowrdCommand => _SetWIFIPasswordCommand = _SetWIFIPasswordCommand ?? new InputCommand<Tuple<string, string>>((tuple) => { _telloController.SetWIFIPassword(tuple.Item1, tuple.Item2); });

        private IInputCommand<Tuple<string, string>> _SetStationModeCommand;
        public IInputCommand<Tuple<string, string>> SetStationModeCommand => _SetStationModeCommand = _SetStationModeCommand ?? new InputCommand<Tuple<string, string>>((tuple) => { _telloController.SetStationMode(tuple.Item1, tuple.Item2); });

        private IInputCommand _GetSpeedCommand;
        public IInputCommand GetSpeedCommand => _GetSpeedCommand = _GetSpeedCommand ?? new InputCommand(_telloController.GetSpeed);

        private IInputCommand _GetBatteryCommand;
        public IInputCommand GetBatteryCommand => _GetBatteryCommand = _GetBatteryCommand ?? new InputCommand(_telloController.GetBattery);

        private IInputCommand _GetTimeCommand;
        public IInputCommand GetTimeCommand => _GetTimeCommand = _GetTimeCommand ?? new InputCommand(_telloController.GetTime);

        private IInputCommand _GetWIFISNRCommand;
        public IInputCommand GetWIFISNRCommand => _GetWIFISNRCommand = _GetWIFISNRCommand ?? new InputCommand(_telloController.GetWIFISNR);

        private IInputCommand _GetSdkVersionCommand;
        public IInputCommand GetSdkVersionCommand => _GetSdkVersionCommand = _GetSdkVersionCommand ?? new InputCommand(_telloController.GetSdkVersion);

        private IInputCommand _GetWIFISerialNumberCommand;
        public IInputCommand GetWiFiSerialNumberCommand => _GetWIFISerialNumberCommand = _GetWIFISerialNumberCommand ?? new InputCommand(_telloController.GetWIFISerialNumber);
#pragma warning restore IDE1006 // Naming Styles

    }
}
