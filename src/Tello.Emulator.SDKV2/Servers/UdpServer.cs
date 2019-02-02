using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Tello.Emulator.SDKV2
{
    internal abstract class UdpServer
    {
        public UdpServer(int port)
        {
#if UWP_CLIENT
            _port = port;

            _running = true;
            RunServer();
#else
            _endpoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), port);
#endif
        }

#if UWP_CLIENT
        private readonly int _port;
        private bool _waiting = true;
#else
        private readonly IPEndPoint _endpoint;
#endif
        private bool _running = false;

        public void Start()
        {
#if UWP_CLIENT
            Debug.WriteLine($"loopback udp server started {_port}");
            _waiting = false;
#else
            if (!_running)
            {
                _running = true;
                RunServer();
            }
#endif
        }

        public void Stop()
        {
#if UWP_CLIENT
            Debug.WriteLine($"loopback udp server stopped {_port}");
            _waiting = true;
#else
            _running = false;
#endif
        }

#if UWP_CLIENT
        private async void RunServer()
        {
            await Task.Run(async () =>
            {
                var wait = new SpinWait();
                IPEndPoint endpoint = null;
                using (var client = new UdpClient(_port))
                {
                    while (_waiting)
                    {
                        // 1. wait for connection from loopback client
                        var datagram = client.Receive(ref endpoint);
                        var message = Encoding.UTF8.GetString(datagram);
                        if (message == "connect")
                        {
                            // return ok
                            datagram = Encoding.UTF8.GetBytes("ok");
                            await client.SendAsync(datagram, datagram.Length, endpoint);

                            Debug.WriteLine($"loopback connected on port {_port}");
                            while (_running)
                            {
                                //Debug.WriteLine($"wait {_waiting} {_port}");
                                if (!_waiting)
                                {
                                    datagram = await GetDatagram();
                                    await client.SendAsync(datagram, datagram.Length, endpoint);
                                }
                            }
                        }
                        else
                        {
                            wait.SpinOnce();
                        }
                    }
                }
            });
        }
#else
        private async void RunServer()
        {
            await Task.Run(async () =>
            {
                var wait = new SpinWait();
                using (var client = new UdpClient())
                {
                    client.Connect(_endpoint);
                    while (_running)
                    {
                        var datagram = await GetDatagram();
                        await client.SendAsync(datagram, datagram.Length);
                        Debug.WriteLine("datagram sent");
                        wait.SpinOnce();
                    }
                }
            });
        }
#endif
        protected abstract Task<byte[]> GetDatagram();
    }
}
