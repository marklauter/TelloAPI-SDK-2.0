using Messenger.Simulator;
using System;
using System.Collections.Generic;
using System.Text;
using Tello.Simulator.Messaging;

namespace Tello.Simulator
{
    public sealed class DroneSimulator
    {
        public DroneSimulator()
        {
            MessageHandler = new DroneMessageHandler();
            (MessageHandler as DroneMessageHandler).CommandReceived += DroneSimulator_CommandReceived;

            StateTransmitter = new StateTransmitter();
            VideoTransmitter = new VideoTransmitter();
        }

        private string DroneSimulator_CommandReceived(Command command)
        {
            try
            {
                // 1. execute the appropriate command simulation
                // 2. update state
                // 3. notify state transmitter
                // 4. compose and return the appropriate command response
                return "ok";
            }
            catch (Exception ex)
            {
                return $"error {ex.GetType().Name}: {ex.Message}";
            }
        }

        public IDroneMessageHandler MessageHandler { get; }
        public IDroneTransmitter StateTransmitter { get; }
        public IDroneTransmitter VideoTransmitter { get; }
    }
}
