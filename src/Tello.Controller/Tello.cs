using Messenger;
using System;
using Tello.Controller.Events;

namespace Tello.Controller
{
    public class Tello : IDisposable
    {
        private readonly ITransceiver _transceiver;
        private readonly IReceiver _stateReceiver;
        private readonly IReceiver _videoReceiver;

        public Tello(
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
        {
            _transceiver = transceiver ?? throw new ArgumentNullException(nameof(transceiver));
            TelloController = new TelloController(_transceiver);
            TelloController.ConnectionStateChanged +=
                (object sender, ConnectionStateChangedArgs e) =>
                {
                    if (e.Connected)
                    {
                        StartLisenters();
                    }
                    else
                    {
                        StopListeners();
                    }
                };

            _stateReceiver = stateReceiver ?? throw new ArgumentNullException(nameof(stateReceiver));
            StateObserver = new StateObserver(_stateReceiver);
            StateObserver.StateChanged += TelloController.UpdateState;

            _videoReceiver = videoReceiver ?? throw new ArgumentNullException(nameof(videoReceiver));
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

        public ITelloController TelloController { get; }
        public IStateObserver StateObserver { get; }
        public IVideoObserver VideoObserver { get; }

        public void Dispose()
        {
            StopListeners();
        }
    }
}
