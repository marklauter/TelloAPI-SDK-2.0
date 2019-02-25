namespace Tello.App.ViewModels
{
    public class ViewModel : PropertyChangedNotifier
    {
        public ViewModel(IUIDispatcher dispatcher) : base(dispatcher) { }

        public void Open(params object[] args)
        {
            Start(args);
        }

        public void Close()
        {
            if (CanClose)
            {
                Finish();
            }
        }

        private bool _canClose = true;
        public bool CanClose { get => _canClose; set => SetProperty(ref _canClose, value); }

        protected virtual void Start(params object[] args) { }
        protected virtual void Finish() { }
    }
}
