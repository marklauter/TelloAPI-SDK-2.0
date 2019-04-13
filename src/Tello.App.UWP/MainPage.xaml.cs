#define NO_EMULATOR

#if EMULATOR
using Tello.Emulator.SDKV2;
#else
using Tello.Udp;
using Windows.UI.Popups;
#endif
using System.Threading;
using Tello.App.MvvM;
using Tello.App.UWP.Services;
using Tello.App.ViewModels;
using Tello.Repository;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace Tello.App.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }
#if EMULATOR
        private readonly TelloEmulator _telloEmulator;
#endif
        private readonly IUIDispatcher _uiDispatcher;
        private readonly IUINotifier _uiNotifier;
        private readonly IRepository _repository;

        public MainPage()
        {
            InitializeComponent();

            _uiDispatcher = new UIDispatcher(SynchronizationContext.Current);
            _uiNotifier = new UINotifier();

            var sessionPrefix = "drone";
#if EMULATOR
            sessionPrefix = "emulator";
#endif
            _repository = new ObservationRepository(sessionPrefix);

#if EMULATOR
            var tello = new TelloEmulator();
            _telloEmulator = tello;
#else
            var tello = new UdpMessenger();
#endif

            ViewModel = new MainViewModel(_uiDispatcher, _uiNotifier, _repository, tello, tello.StateServer, tello.VideoServer, tello.VideoSampleProvider);
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

        private void PowerUpButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if EMULATOR
            _telloEmulator.PowerOn();
            ViewModel.ControllerViewModel.ControlLog.Insert(0, "Tello SDK Emulator Powered On");
#else
            var dialog = new MessageDialog("This method is for the SDK emulator.", "Tello Drone");
            dialog.ShowAsync();
#endif
        }

        private void PowerDownButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if EMULATOR
            _telloEmulator.PowerOff();
            ViewModel.ControllerViewModel.ControlLog.Insert(0, "Tello SDK Emulator Powered Off");
#else
            var dialog = new MessageDialog("This method is for the SDK emulator.", "Tello Drone");
            dialog.ShowAsync();
#endif
        }
    }
}
