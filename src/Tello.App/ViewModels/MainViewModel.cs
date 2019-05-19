using Messenger;
using Repository;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Entities.Sqlite;

namespace Tello.App.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public TelloControllerViewModel ControllerViewModel { get; }
        public TelloStateViewModel StateViewModel { get; }
        public TelloVideoViewModel VideoViewModel { get; }

        private readonly DroneMessenger _tello;

        public MainViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IRepository repository,
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
                : base(dispatcher, notifier)
        {
            _tello = new DroneMessenger(transceiver, stateReceiver, videoReceiver);

            repository.CreateCatalog<Session>();
            repository.CreateCatalog<ObservationGroup>();
            repository.CreateCatalog<StateObservation>();
            repository.CreateCatalog<AirSpeedObservation>();
            repository.CreateCatalog<AttitudeObservation>();
            repository.CreateCatalog<BatteryObservation>();
            repository.CreateCatalog<HobbsMeterObservation>();
            repository.CreateCatalog<PositionObservation>();
            repository.CreateCatalog<ResponseObservation>();

            var session = repository.NewEntity<Session>();

            StateViewModel = new TelloStateViewModel(
                dispatcher,
                notifier,
                _tello.StateObserver,
                repository,
                session);

            VideoViewModel = new TelloVideoViewModel(
                dispatcher,
                notifier,
                _tello.VideoObserver);

            ControllerViewModel = new TelloControllerViewModel(
                dispatcher, 
                notifier, 
                _tello.Controller, 
                repository,
                session);
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            StateViewModel.Open(args);
            VideoViewModel.Open(args);
            ControllerViewModel.Open(args);
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            args.CanClose = ControllerViewModel.Close() && StateViewModel.Close() && VideoViewModel.Close();
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
