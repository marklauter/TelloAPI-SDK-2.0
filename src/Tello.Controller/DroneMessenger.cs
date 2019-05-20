using Messenger;
using System;
using Tello.Events;

namespace Tello.Controller
{
    public sealed class DroneMessenger
    {
        private readonly ITransceiver _transceiver;
        private readonly IReceiver _stateReceiver;
        private readonly IReceiver _videoReceiver;

        public DroneMessenger(
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
        {
            _transceiver = transceiver ?? throw new ArgumentNullException(nameof(transceiver));
            _stateReceiver = stateReceiver ?? throw new ArgumentNullException(nameof(stateReceiver));
            _videoReceiver = videoReceiver ?? throw new ArgumentNullException(nameof(videoReceiver));

            Controller = new FlightController(_transceiver);

            Controller.ConnectionStateChanged +=
                (object sender, ConnectionStateChangedArgs e) =>
                {
                    if (e.IsConnected)
                    {
                        StartLisenters();
                    }
                    else
                    {
                        StopListeners();
                    }
                };

            StateObserver = new StateObserver(_stateReceiver);
            StateObserver.StateChanged += Controller.UpdateState;
            Controller.PositionChanged += StateObserver.UpdatePosition;

            VideoObserver = new VideoObserver(_videoReceiver);
        }


        #region Listeners

        private void StartLisenters()
        {
            _stateReceiver.Start();
            _videoReceiver.Start();
        }

        private void StopListeners()
        {
            _stateReceiver.Stop();
            _videoReceiver.Stop();
        }

        #endregion

        public IFlightController Controller { get; }
        public IStateObserver StateObserver { get; }
        public IVideoObserver VideoObserver { get; }
    }
}
