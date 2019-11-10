// <copyright file="MainViewModel.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

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

        private readonly DroneMessenger tello;

        public MainViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IRepository repository,
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
                : base(dispatcher, notifier)
        {
            this.tello = new DroneMessenger(transceiver, stateReceiver, videoReceiver);

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

            this.StateViewModel = new TelloStateViewModel(
                dispatcher,
                notifier,
                this.tello.StateObserver,
                repository,
                session);

            this.VideoViewModel = new TelloVideoViewModel(
                dispatcher,
                notifier,
                this.tello.VideoObserver);

            this.ControllerViewModel = new TelloControllerViewModel(
                dispatcher,
                notifier,
                this.tello.Controller,
                repository,
                session);
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            this.StateViewModel.Open(args);
            this.VideoViewModel.Open(args);
            this.ControllerViewModel.Open(args);
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            args.CanClose = this.ControllerViewModel.Close() && this.StateViewModel.Close() && this.VideoViewModel.Close();
        }

        internal void ClearDatabase()
        {
            this.StateViewModel.ClearDatabase();
            this.ControllerViewModel.ClearDatabase();
        }

        private IInputCommand clearDatabaseCommand;

        public IInputCommand ClearDatabaseCommand => this.clearDatabaseCommand = this.clearDatabaseCommand ?? new InputCommand(this.ClearDatabase);
    }
}
