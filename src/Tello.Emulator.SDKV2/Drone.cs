using System;
using System.Text;
using System.Threading;
using Tello.Udp;

namespace Tello.Emulator.SDKV2
{
    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf
    //notes:
    // video reports on 11111
    // state reports on 8890
    // command listener receives on 8889

    public class Drone
    {
        public Drone(ILog log, byte[] videoData, Sample[] sampleDefs)
        {
            _log = log;

            _udpReceiver = new Listener(8889);
            _udpReceiver.DatagramReceived += _udpReceiver_DatagramReceived;

            _droneState = new DroneState();
            _stateServer = new StateServer(8890, _droneState);
            _videoServer = new VideoServer(11111, videoData, sampleDefs);

            _commandInterpreter = new CommandInterpreter(_droneState, _videoServer, _stateServer, log);

            _batteryTimer = new Timer(UpdateBattery, null, 10000, 10000);
        }

        private void UpdateBattery(object state)
        {
            if (_poweredOn)
            {
                // documentation says there's ~ 15 minuts of battery
                _droneState.BatteryPercentage = 100 - (int)((DateTime.Now - _poweredOnTime).TotalMinutes / 15.0 * 100);
                Log($"battery updated {_droneState.BatteryPercentage}");
                if (_droneState.BatteryPercentage < 1)
                {
                    PowerOff();
                    Log("battery died");
                }
            }
        }

        private readonly ILog _log;
        private readonly Timer _batteryTimer;
        private bool _poweredOn = false;
        private DateTime _poweredOnTime;
        private readonly Listener _udpReceiver;
        private readonly DroneState _droneState;
        private readonly VideoServer _videoServer;
        private readonly StateServer _stateServer;
        private readonly CommandInterpreter _commandInterpreter;

        private void _udpReceiver_DatagramReceived(object sender, DatagramReceivedArgs e)
        {
            var message = Encoding.UTF8.GetString(e.Datagram);
            var response = _commandInterpreter.Interpret(message);
            Log($"message: {message}, response: {response}");
            if (!String.IsNullOrEmpty(response))
            {
                e.Reply = Encoding.UTF8.GetBytes(response);
            }
        }

        public void PowerOn()
        {
            if (!_poweredOn)
            {
                _poweredOn = true;
                _udpReceiver.Start();
                _poweredOnTime = DateTime.Now;
                Log("drone powered on");
            }
        }

        public void PowerOff()
        {
            if (_poweredOn)
            {
                _poweredOn = false;
                _udpReceiver.Stop();
                Log("drone powered off");
            }
        }

        public void Log(string meesage)
        {
            if (_log != null)
            {
                _log.WriteLine(meesage);
            }
        }
    }
}
