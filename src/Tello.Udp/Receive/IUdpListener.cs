using System;

namespace Tello.Udp
{
    public interface IUdpListener
    {
        int Port { get; }
        bool IsActive { get; }

        void Start();
        void Stop();

        event EventHandler<DatagramReceivedArgs> DatagramReceived;
    }
}
