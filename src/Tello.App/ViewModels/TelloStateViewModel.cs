using System;
using System.Collections.ObjectModel;
using Tello.Controller;
using Tello.Messaging;

namespace Tello.App.ViewModels
{
    public class TelloStateViewModel : ViewModel
    {
        private readonly ITelloStateChangedNotifier _telloStateChangedNotifier;

        public TelloStateViewModel(IUIDispatcher dispatcher, ITelloStateChangedNotifier telloStateChangedNotifier) : base(dispatcher)
        {
            _telloStateChangedNotifier = telloStateChangedNotifier ?? throw new ArgumentNullException(nameof(telloStateChangedNotifier));
        }

        protected override void Start(params object[] args)
        {
            _telloStateChangedNotifier.TelloStateChanged += OnTelloStateChanged;
        }

        protected override void Finish()
        {
            _telloStateChangedNotifier.TelloStateChanged -= OnTelloStateChanged;
        }

        private void OnTelloStateChanged(object sender, TelloStateChangedArgs e)
        {
            Dispatcher.Invoke(() => { _stateHistory.Add(State); });
            State = e.State;
        }

        public ObservableCollection<ITelloState> _stateHistory = new ObservableCollection<ITelloState>();

        private ITelloState _state;
        public ITelloState State { get => _state; set => SetProperty(ref _state, value); }
    }
}
