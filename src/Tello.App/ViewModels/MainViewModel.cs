using System;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Messaging;
using Tello.Repository;

namespace Tello.App.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public TelloControllerViewModel ControllerViewModel { get; }
        public TelloStateViewModel StateViewModel { get; }

        private readonly FlightController _flightController;
        private readonly ITrackingSession _repository;

        public MainViewModel(
            IUIDispatcher dispatcher,
            IUINotifier uiNotifier,
            ITrackingSession repository,
            IMessengerService messenger,
            IMessageRelayService<IRawDroneState> stateServer,
            IMessageRelayService<IVideoSample> videoServer,
            IVideoSampleProvider videoSampleProvider) : base(dispatcher, uiNotifier)
        {
            #region argument validation
            if (messenger == null)
            {
                throw new ArgumentNullException(nameof(messenger));
            }

            if (stateServer == null)
            {
                throw new ArgumentNullException(nameof(stateServer));
            }

            if (videoServer == null)
            {
                throw new ArgumentNullException(nameof(videoServer));
            }

            if (videoSampleProvider == null)
            {
                throw new ArgumentNullException(nameof(videoSampleProvider));
            }
            #endregion
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));

            _flightController = new FlightController(messenger, stateServer, videoServer, videoSampleProvider);
            ControllerViewModel = new TelloControllerViewModel(dispatcher, uiNotifier, _flightController, repository);
            StateViewModel = new TelloStateViewModel(dispatcher, uiNotifier, _flightController, repository);
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            ControllerViewModel.Open(args);
            StateViewModel.Open(args);
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            args.CanClose = ControllerViewModel.Close() && StateViewModel.Close();
        }

        internal void ClearDatabase()
        {
            StateViewModel.ClearDatabase();
            ControllerViewModel.ClearDatabase();
        }

        private IInputCommand _clearDatabaseCommand;
        public IInputCommand ClearDatabaseCommand => _clearDatabaseCommand = _clearDatabaseCommand ?? new InputCommand(ClearDatabase);
    }
}
