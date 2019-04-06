using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Messaging;
using Tello.Repository;

namespace Tello.App.ViewModels
{
    //https://www.actiprosoftware.com/products/controls/wpf/gauge
    public class TelloStateViewModel : ViewModel
    {
        private readonly IStateChangedNotifier _telloStateChangedNotifier;
        private readonly IRepository _repository;

        public TelloStateViewModel(IUIDispatcher dispatcher, IStateChangedNotifier telloStateChangedNotifier, IRepository repository) : base(dispatcher)
        {
            _telloStateChangedNotifier = telloStateChangedNotifier ?? throw new ArgumentNullException(nameof(telloStateChangedNotifier));
            _repository = repository;
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _telloStateChangedNotifier.StateChanged += OnTelloStateChanged;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _telloStateChangedNotifier.StateChanged -= OnTelloStateChanged;
        }

        private async void OnTelloStateChanged(object sender, StateChangedArgs e)
        {
            Dispatcher.Invoke((state)=> { _stateHistory.Add(state as ITelloState); }, State);
            State = e.State;
            if(_repository != null)
            {
                var groupId = Guid.NewGuid().ToString();
                await _repository.Write(new PositionObservation(e.State, groupId));
                await _repository.Write(new AttitudeObservation(e.State, groupId));
                await _repository.Write(new AirSpeedObservation(e.State, groupId));
                await _repository.Write(new BatteryObservation(e.State, groupId));
                await _repository.Write(new HobbsMeterObservation(e.State, groupId));
            }
        }

        public ObservableCollection<ITelloState> _stateHistory = new ObservableCollection<ITelloState>();

        private ITelloState _state;
        public ITelloState State { get => _state; set => SetProperty(ref _state, value); }

    }
}
