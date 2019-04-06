using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Tello.App.MvvM;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.App.ViewModels
{
    //https://www.actiprosoftware.com/products/controls/wpf/gauge
    public class TelloStateViewModel : ViewModel
    {
        private readonly IStateChangedNotifier _telloStateChangedNotifier;

        public TelloStateViewModel(IUIDispatcher dispatcher, IStateChangedNotifier telloStateChangedNotifier) : base(dispatcher)
        {
            _telloStateChangedNotifier = telloStateChangedNotifier ?? throw new ArgumentNullException(nameof(telloStateChangedNotifier));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            _telloStateChangedNotifier.StateChanged += OnTelloStateChanged;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            _telloStateChangedNotifier.StateChanged -= OnTelloStateChanged;
        }

        private void OnTelloStateChanged(object sender, StateChangedArgs e)
        {
            Dispatcher.Invoke((state)=> { _stateHistory.Add(state as ITelloState); }, State);
            State = e.State;
        }

        public ObservableCollection<ITelloState> _stateHistory = new ObservableCollection<ITelloState>();

        private ITelloState _state;
        public ITelloState State { get => _state; set => SetProperty(ref _state, value); }

    }
}
