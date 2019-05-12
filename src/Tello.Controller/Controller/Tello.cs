using Messenger;
using Messenger.Tello;
using System;
using System.Threading.Tasks;

namespace Tello.Controller
{

    //public abstract class EnvelopeObserver : Observer<IEnvelope>
    //{

    //}

    public sealed class Tello : Observer<IResponse<string>>, ITello
    {
        private readonly TelloMessenger _messenger;
        private readonly IReceiver _stateReceiver;
        private readonly IReceiver _videoReceiver;

        public Tello(
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
            : base()
        {
            _messenger = new TelloMessenger(transceiver ?? throw new ArgumentNullException(nameof(transceiver)));
            Subscribe(_messenger);

            _stateReceiver = stateReceiver ?? throw new ArgumentNullException(nameof(stateReceiver));
            _videoReceiver = videoReceiver ?? throw new ArgumentNullException(nameof(videoReceiver));
        }

        #region Observer<IResponse<string>>
        public override void OnError(Exception error)
        {
            throw new NotImplementedException();
        }

        public override void OnNext(IResponse<string> response)
        {
            throw new NotImplementedException();
            //var observation = new ResponseObservation(group, response);
            //_repository.Insert(observation);
        }
        #endregion

        #region ITello

        private bool _connected = false;

        public async Task Connect()
        {
            if (!_connected)
            {
                var response = await _messenger.SendAsync(Commands.EnterSdkMode);
                _connected = response != null && response.Success;
                StartLisenters();
            }
        }

        private void StartLisenters()
        {
            _stateReceiver.Start();
            _videoReceiver.Start();
        }

        public void Disconnect()
        {
            if (_connected)
            {
                StopListeners();
                _connected = false;
            }
        }

        private void StopListeners()
        {
            _stateReceiver.Stop();
            _videoReceiver.Stop();
        }

        public async Task EmergencyStop()
        {
            await _messenger.SendAsync(Commands.EmergencyStop);
            Disconnect();
        }

        public async Task Stop()
        {
            await _messenger.SendAsync(Commands.Stop);
        }

        public void Curve(int x1, int y1, int z1, int x2, int y2, int z2, int speed)
        {
            _messenger.SendAsync(Commands.Curve, x1, y1, z1, x2, y2, z2, speed);
        }

        public void Flip(CardinalDirections direction)
        {
            _messenger.SendAsync(Commands.Flip, (char)(CardinalDirection)direction);
        }

        public void FlyPolygon(int sides, int length, int speed, ClockDirections clockDirection, bool land = false)
        {
            throw new NotImplementedException();
        }

        public void GetBattery()
        {
            _messenger.SendAsync(Commands.GetBattery);
        }

        public void GetSdkVersion()
        {
            _messenger.SendAsync(Commands.GetSdkVersion);
        }

        public void GetSpeed()
        {
            _messenger.SendAsync(Commands.GetSpeed);
        }

        public void GetTime()
        {
            _messenger.SendAsync(Commands.GetTime);
        }

        public void GetSerialNumber()
        {
            _messenger.SendAsync(Commands.GetSerialNumber);
        }

        public void GetWIFISNR()
        {
            _messenger.SendAsync(Commands.GetWIFISnr);
        }

        public void Go(int x, int y, int z, int speed)
        {
            _messenger.SendAsync(Commands.Go, x, y, z, speed);
        }

        public void GoBackward(int cm)
        {
            throw new NotImplementedException();
        }

        public void GoDown(int cm)
        {
            throw new NotImplementedException();
        }

        public void GoForward(int cm)
        {
            throw new NotImplementedException();
        }

        public void GoLeft(int cm)
        {
            throw new NotImplementedException();
        }

        public void GoRight(int cm)
        {
            throw new NotImplementedException();
        }

        public void GoUp(int cm)
        {
            throw new NotImplementedException();
        }

        public void Land()
        {
            throw new NotImplementedException();
        }

        public void Set4ChannelRC(int leftRight, int forwardBackward, int upDown, int yaw)
        {
            throw new NotImplementedException();
        }

        public void SetHeight(int cm)
        {
            throw new NotImplementedException();
        }

        public void SetSpeed(int speed)
        {
            throw new NotImplementedException();
        }

        public void SetStationMode(string ssid, string password)
        {
            throw new NotImplementedException();
        }

        public void SetWIFIPassword(string ssid, string password)
        {
            throw new NotImplementedException();
        }

        public void StartVideo()
        {
            throw new NotImplementedException();
        }

        public void StopVideo()
        {
            throw new NotImplementedException();
        }

        public void TakeOff()
        {
            throw new NotImplementedException();
        }

        public void TurnClockwise(int degress)
        {
            throw new NotImplementedException();
        }

        public void TurnCounterClockwise(int degress)
        {
            throw new NotImplementedException();
        }

        public void TurnLeft(int degress)
        {
            throw new NotImplementedException();
        }

        public void TurnRight(int degress)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
