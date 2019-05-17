using Messenger;
using Repository;
using System;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Entities;
using Tello.Entities.Sqlite;

namespace Tello.App.ViewModels
{
    public class MainViewModel : ViewModel
    {
        public TelloControllerViewModel ControllerViewModel { get; }
        public TelloStateViewModel StateViewModel { get; }

        private readonly DroneMessenger _tello;

        public MainViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IRepository repository,
            IReceiver stateReceiver,
            IReceiver videoReceiver,
            ITransceiver transceiver)
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

            //            ControllerViewModel = new TelloControllerViewModel(dispatcher, notifier, _flightController, repository);
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
