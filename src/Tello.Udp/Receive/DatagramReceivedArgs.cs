using System;
using System.Net;

namespace Tello.Udp
{
    public sealed class DatagramReceivedArgs : EventArgs
    {
        public DatagramReceivedArgs(byte[] datagram, IPEndPoint remoteEndpoint)
        {
            Datagram = datagram;
            RemoteEndpoint = remoteEndpoint;
        }
        public byte[] Datagram { get; }
        public IPEndPoint RemoteEndpoint { get; }

        /// <summary>
        /// set reply inside the received event to send a message back to tello
        /// </summary>
        public byte[] Reply { get; set; }
    }
}
