
using Tello.Controller;

namespace Tello.App.ViewModels
{
    public class TelloControllerViewModel : ViewModel
    {
        private readonly ITelloController _telloController;

        public TelloControllerViewModel(IUIDispatcher dispatcher, ITelloController telloController) : base(dispatcher)
        {
            _telloController = telloController;
        }

        protected override void Start(params object[] args)
        {
            _telloController.ExceptionThrown += OnExceptionThrown;
            _telloController.CommandExceptionThrown += OnCommandExceptionThrown;
            _telloController.QueryResponseReceived += OnQueryResponseReceived;
            _telloController.CommandResponseReceived += OnCommandResponseReceived;
        }

        protected override void Finish()
        {
            _telloController.ExceptionThrown -= OnExceptionThrown;
            _telloController.CommandExceptionThrown -= OnCommandExceptionThrown;
            _telloController.QueryResponseReceived -= OnQueryResponseReceived;
            _telloController.CommandResponseReceived -= OnCommandResponseReceived;
        }

        private void OnCommandExceptionThrown(object sender, CommandExceptionThrownArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnExceptionThrown(object sender, ExceptionThrownArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnCommandResponseReceived(object sender, CommandResponseReceivedArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnQueryResponseReceived(object sender, QueryResponseReceivedArgs e)
        {
            throw new System.NotImplementedException();
        }

        private IInputCommand _enterSdkModeCommand;
        public IInputCommand EnterSdkModeCommand => _enterSdkModeCommand = _enterSdkModeCommand ?? new InputCommand(_telloController.EnterSdkMode);

        private IInputCommand _takeOffCommand;
        public IInputCommand TakeOffCommand => _takeOffCommand = _takeOffCommand ?? new InputCommand(_telloController.TakeOff);

        private IInputCommand _landCommand;
        public IInputCommand LandCommand => _landCommand = _landCommand ?? new InputCommand(_telloController.Land);

        private IInputCommand _stopCommand;
        public IInputCommand StopCommand => _stopCommand = _stopCommand ?? new InputCommand(_telloController.Stop);

        private IInputCommand _emergencyStopCommand;
        public IInputCommand EmergencyStopCommand => _emergencyStopCommand = _emergencyStopCommand ?? new InputCommand(_telloController.EmergencyStop);

        private IInputCommand _startVideoCommand;
        public IInputCommand StartVideoCommand => _startVideoCommand = _startVideoCommand ?? new InputCommand(_telloController.StartVideo);

        private IInputCommand _stopVideoCommand;
        public IInputCommand StopVideoCommand => _stopVideoCommand = _stopVideoCommand ?? new InputCommand(_telloController.StopVideo);


    }
}
