#define EMULATOR

#if EMULATOR
using Tello.Emulator.SDKV2;
#else
using Tello.Udp;
#endif
using System;
using System.Threading;
using Tello.App.MvvM;
using Tello.App.UWP.Services;
using Tello.App.ViewModels;
using Tello.Messaging;
using Tello.Repository;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;
using Windows.UI.Popups;

namespace Tello.App.UWP
{
    public sealed partial class MainPage : Page
    {
        public MainViewModel ViewModel { get; }
#if EMULATOR
        private readonly IMessengerService _tello;
#endif
        private readonly IUIDispatcher _uiDispatcher;
        private readonly IUINotifier _uiNotifier;
        private readonly IRepository _repository;

        public MainPage()
        {
            InitializeComponent();

            _uiDispatcher = new UIDispatcher(SynchronizationContext.Current);
            _uiNotifier = new UINotifier();
            _repository = new ObservationRepository();

#if EMULATOR
            var tello = new TelloEmulator();
            _tello = tello;
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

        private async void PowerUpButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if EMULATOR
            (_tello as TelloEmulator).PowerOn();
            var dialog = new MessageDialog("Powered On", "Tello SDK Emulator");
#else
            var dialog = new MessageDialog("This method is for the SDK emulator.", "Tello Drone");
#endif
            await dialog.ShowAsync();
        }

        private async void PowerDownButton_Click(object sender, Windows.UI.Xaml.RoutedEventArgs e)
        {
#if EMULATOR
            (_tello as TelloEmulator).PowerOff();
            var dialog = new MessageDialog("Powered Off", "Tello SDK Emulator");
#else
            var dialog = new MessageDialog("This method is for the SDK emulator.", "Tello Drone");
#endif
            await dialog.ShowAsync();
        }
    }
}
