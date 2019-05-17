#define EMULATOR

#if EMULATOR
using Messenger.Simulator;
using Tello.Simulator;
#else
using Messenger.Udp;
using System.Net;
#endif

using Repository.Sqlite;
using System.Threading;
using Tello.App.MvvM;
using Tello.App.UWP.Services;
using Tello.App.ViewModels;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tello.App.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }

        private readonly IUIDispatcher _dispatcher;
        private readonly IUINotifier _notifier;
#if EMULATOR
        private DroneSimulator _simulator;
#endif

        private MainViewModel CreateMainViewModel(IUIDispatcher dispatcher, IUINotifier notifier)
        {
#if EMULATOR
            _simulator = new DroneSimulator();
            var transceiver = new SimTransceiver(_simulator.MessageHandler);
            var stateReceiver = new SimReceiver(_simulator.StateTransmitter);
            var videoReceiver = new SimReceiver(_simulator.VideoTransmitter);
#else
            var transceiver = new UdpTransceiver(IPAddress.Parse("192.168.10.1"), 8889);
            var stateReceiver = new UdpReceiver(8890);
            var videoReceiver = new UdpReceiver(11111);
#endif
            return new MainViewModel(
                _dispatcher,
                _notifier,
                new SqliteRepository((null, "tello.sqlite")),
                transceiver,
                stateReceiver,
                videoReceiver);
        }

        public MainPage()
        {
            InitializeComponent();

            _dispatcher = new UIDispatcher(SynchronizationContext.Current);
            _notifier = new UINotifier();

            ViewModel = CreateMainViewModel(_dispatcher, _notifier);
            DataContext = ViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            ViewModel.Open();
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ViewModel.Close();

            base.OnNavigatedFrom(e);
        }
    }
}
