using System;
using System.Diagnostics;
using Tello.Udp;

namespace Tello.Controller.State
{
    internal sealed class StateServer
    {
        public StateServer(int port = 8890)
        {
            _listener = new Listener(port);
            _listener.DatagramReceived += _listener_DatagramReceived;
            _listener.Start();
            Debug.WriteLine($"{nameof(StateServer)} receiving on port {8890}");
        }

        public event EventHandler<DroneStateReceivedArgs> DroneStateReceived;

        private readonly Listener _listener;

        private void _listener_DatagramReceived(object sender, DatagramReceivedArgs e)
        {
            if (e.Datagram != null)
            {
                DroneStateReceived?.Invoke(this, new DroneStateReceivedArgs(DroneState.FromDatagram(e.Datagram)));
            }
        }
    }
}
