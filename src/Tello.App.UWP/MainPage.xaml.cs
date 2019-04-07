#define EMULATOR

#if EMULATOR
using Tello.Emulator.SDKV2;
#else
using Tello.Udp;
#endif

using System.Threading;
using Tello.App.MvvM;
using Tello.App.UWP.Services;
using Tello.App.ViewModels;
using Tello.Controller;
using Tello.Messaging;
using Tello.Repository;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace Tello.App.UWP
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private readonly IMessengerService _tello;
        private readonly FlightController _flightController;
        private readonly IUIDispatcher _uiDispatcher;
        private readonly IUserNotifier _userNotifier;
        private readonly IRepository _repository;

        public TelloControllerViewModel ControllerViewModel { get; }
        public TelloStateViewModel StateViewModel { get; }

        public MainPage()
        {
            InitializeComponent();

            DataContext = this;

            _uiDispatcher = new UIDispatcher(SynchronizationContext.Current);
            _userNotifier = new UserNotifier();
            _repository = new ObservationRepository();

#if EMULATOR
            var tello = new TelloEmulator();
#else
            var tello = new UdpMessenger();
#endif
            _tello = tello;

            _flightController = new FlightController(tello, tello.StateServer, tello.VideoServer, tello.VideoSampleProvider);

            ControllerViewModel = new TelloControllerViewModel(_uiDispatcher, _userNotifier, _flightController);
            StateViewModel = new TelloStateViewModel(_uiDispatcher, _userNotifier, _flightController, _repository);

            ControllerGrid.DataContext = ControllerViewModel;
            StateGrid.DataContext = StateViewModel;
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

#if EMULATOR
            (_tello as TelloEmulator).PowerOn();
#endif

            ControllerViewModel.Open(new OpenEventArgs());
            StateViewModel.Open(new OpenEventArgs());
        }

        protected override void OnNavigatedFrom(NavigationEventArgs e)
        {
            ControllerViewModel.Close();
            StateViewModel.Close();

#if EMULATOR
            (_tello as TelloEmulator).PowerOff();
#endif

            base.OnNavigatedFrom(e);
        }
    }
}
