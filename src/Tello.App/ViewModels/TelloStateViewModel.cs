using System;
using System.Collections.ObjectModel;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.App.ViewModels
{
    public class TelloStateViewModel : ViewModel
    {
        private readonly IStateChangedNotifier _telloStateChangedNotifier;

        public TelloStateViewModel(IUIDispatcher dispatcher, IStateChangedNotifier telloStateChangedNotifier) : base(dispatcher)
        {
            _telloStateChangedNotifier = telloStateChangedNotifier ?? throw new ArgumentNullException(nameof(telloStateChangedNotifier));
        }

        protected override void Start(params object[] args)
        {
            _telloStateChangedNotifier.StateChanged += OnTelloStateChanged;
        }

        protected override void Finish()
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
