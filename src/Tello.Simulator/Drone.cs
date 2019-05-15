using System;
using System.Text;
using System.Threading.Tasks;
using Tello.Simulator.Transmitters;

namespace Tello.Simulator
{
    public sealed class Drone
    {
        private readonly StateTransmitter _stateTransmitter;
        private readonly VideoTransmitter _videoTransmitter;

        public Drone(StateTransmitter stateTransmitter, VideoTransmitter videoTransmitter)
        {
            _stateTransmitter = stateTransmitter ?? throw new ArgumentNullException(nameof(stateTransmitter));
            _videoTransmitter = videoTransmitter ?? throw new ArgumentNullException(nameof(videoTransmitter));
        }

        public Task<byte[]> Invoke(byte[] buffer)
        {
            if (buffer != null)
            {
                // 1. parse the command string 
                var command = (Command)buffer;
                
                // 2. execute the appropriate command simulation
                // 3. update state
                // 4. notify state transmitter
                // 5. compose and return the appropriate command response
                return Task.FromResult(Encoding.UTF8.GetBytes("ok"));
            }
            else
            {
                return Task.FromResult(Encoding.UTF8.GetBytes("error"));
            }
        }
    }
}
