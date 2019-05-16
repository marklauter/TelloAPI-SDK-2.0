using Messenger.Simulator;
using System;
using System.Text;
using System.Threading.Tasks;

namespace Tello.Simulator.Messaging
{
    internal sealed class DroneMessageHandler : IDroneMessageHandler
    {
        public DroneMessageHandler(Func<Command, string> commandReceived)
        {
            _commandReceived = commandReceived ?? throw new ArgumentNullException(nameof(commandReceived));
        }

        private readonly Func<Command, string> _commandReceived;

        public Task<byte[]> Invoke(byte[] buffer)
        {
            if (buffer != null)
            {
                var result = _commandReceived((Command)buffer);
                return Task.FromResult(Encoding.UTF8.GetBytes(result));
            }
            else
            {
                return Task.FromResult(Encoding.UTF8.GetBytes("error"));
            }
        }
    }
}


