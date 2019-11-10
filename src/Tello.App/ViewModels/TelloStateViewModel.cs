// <copyright file="TelloStateViewModel.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using Repository;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Entities;
using Tello.Entities.Sqlite;
using Tello.State;

namespace Tello.App.ViewModels
{
    // https://www.actiprosoftware.com/products/controls/wpf/gauge
    // https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/radialgauge
    public class TelloStateViewModel : ViewModel
    {
        private readonly IStateObserver stateObserver;
        private readonly IRepository repository;
        private readonly ISession session;

        public TelloStateViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IStateObserver stateObserver,
            IRepository repository,
            ISession session)
            : base(dispatcher, notifier)
        {
            this.stateObserver = stateObserver ?? throw new ArgumentNullException(nameof(stateObserver));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            this.stateObserver.StateChanged += this.StateChanged;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            this.stateObserver.StateChanged -= this.StateChanged;
        }

        private void StateChanged(object sender, Events.StateChangedArgs e)
        {
            // todo: this should be pushed directly to a queue to minimize time in method. the queue can be picked up by a processor that does what this method is currently doing.
            this.Dispatcher.Invoke(
                (state) =>
                {
                    this.State = state as ITelloState;
                    this.StateHistory.Add(state as ITelloState);
                    if (this.StateHistory.Count > 500)
                    {
                        this.StateHistory.RemoveAt(0);
                    }
                },
                e.State);

            var group = this.repository.NewEntity<ObservationGroup>(this.session);
            this.repository.Insert(new StateObservation(group, e.State));
            this.repository.Insert(new AirSpeedObservation(group, e.State));
            this.repository.Insert(new AttitudeObservation(group, e.State));
            this.repository.Insert(new BatteryObservation(group, e.State));
            this.repository.Insert(new HobbsMeterObservation(group, e.State));
            this.repository.Insert(new PositionObservation(group, e.State));
        }

        public ObservableCollection<ITelloState> StateHistory { get; } = new ObservableCollection<ITelloState>();

        private ITelloState state;

        public ITelloState State
        {
            get => this.state;
            set => this.SetProperty(ref this.state, value);
        }

        internal void ClearDatabase()
        {
            if (this.repository != null)
            {
                this.repository.Delete<Session>();
                this.repository.Delete<ObservationGroup>();
                this.repository.Delete<StateObservation>();
                this.repository.Delete<AirSpeedObservation>();
                this.repository.Delete<AttitudeObservation>();
                this.repository.Delete<BatteryObservation>();
                this.repository.Delete<HobbsMeterObservation>();
                this.repository.Delete<PositionObservation>();
                this.repository.Delete<ResponseObservation>();
            }
        }
    }
}
