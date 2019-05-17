using Repository;
using System;
using System.Collections.ObjectModel;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Entities;
using Tello.Entities.Sqlite;
using Tello.State;

namespace Tello.App.ViewModels
{
    //https://www.actiprosoftware.com/products/controls/wpf/gauge
    //https://docs.microsoft.com/en-us/windows/communitytoolkit/controls/radialgauge
    public class TelloStateViewModel : ViewModel
    {
        private readonly IStateObserver _stateObserver;
        private readonly IRepository _repository;
        private readonly ISession _session;

        public TelloStateViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IStateObserver stateObserver,
            IRepository repository,
            ISession session)
            : base(dispatcher, notifier)
        {
            _stateObserver = stateObserver ?? throw new ArgumentNullException(nameof(stateObserver));
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _stateObserver.StateChanged += StateChanged;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _stateObserver.StateChanged -= StateChanged;
        }

        private void StateChanged(object sender, Events.StateChangedArgs e)
        {
            //todo: this should be pushed directly to a queue to minimize time in method. the queue can be picked up by a processor that does what this method is currently doing.
            Dispatcher.Invoke((state) =>
            {
                State = state as ITelloState;
                StateHistory.Add(state as ITelloState);
                if (StateHistory.Count > 500)
                {
                    StateHistory.RemoveAt(0);
                }
            },
            e.State);


            var group = _repository.NewEntity<ObservationGroup>(_session);
            _repository.Insert(new StateObservation(group, e.State));
            _repository.Insert(new AirSpeedObservation(group, e.State));
            _repository.Insert(new AttitudeObservation(group, e.State));
            _repository.Insert(new BatteryObservation(group, e.State));
            _repository.Insert(new HobbsMeterObservation(group, e.State));
            _repository.Insert(new PositionObservation(group, e.State));
        }

        public ObservableCollection<ITelloState> StateHistory { get; } = new ObservableCollection<ITelloState>();

        private ITelloState _state;
        public ITelloState State
        {
            get => _state;
            set => SetProperty(ref _state, value);
        }

        internal void ClearDatabase()
        {
            if (_repository != null)
            {
                _repository.Delete<Session>();
                _repository.Delete<ObservationGroup>();
                _repository.Delete<StateObservation>();
                _repository.Delete<AirSpeedObservation>();
                _repository.Delete<AttitudeObservation>();
                _repository.Delete<BatteryObservation>();
                _repository.Delete<HobbsMeterObservation>();
                _repository.Delete<PositionObservation>();
                _repository.Delete<ResponseObservation>();
            }
        }
    }
}
