// <copyright file="DroneSimulator.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Messenger.Simulator;
using Tello.Simulator.Messaging;
using Tello.State;

namespace Tello.Simulator
{
    public sealed class DroneSimulator
    {
        // todo: 1. execute the appropriate command simulation
        // todo: 2. update state
        // todo: 3. notify state transmitter
        // todo: 4. compose and return the appropriate command response
        private bool isVideoStreaming = false;
        private bool isFlying = false;
        private Vector position = new Vector();
        private ITelloState state = new TelloState();
        private int speed = 0;
        private int height = 0;
        private bool inCommandMode = false;

        public DroneSimulator()
        {
            this.MessageHandler = new DroneMessageHandler(this.DroneSimulator_CommandReceived);
            this.StateTransmitter = new StateTransmitter();
            this.VideoTransmitter = new VideoTransmitter();

            this.VideoThread();
            this.StateThread();
        }

        public IDroneMessageHandler MessageHandler { get; }

        public IDroneTransmitter StateTransmitter { get; }

        public IDroneTransmitter VideoTransmitter { get; }

        private async void VideoThread()
        {
            var bytes = Encoding.UTF8.GetBytes("this is fake video data");
            await Task.Run(async () =>
            {
                var spinWait = default(SpinWait);
                while (true)
                {
                    if (this.isVideoStreaming)
                    {
                        // (VideoTransmitter as VideoTransmitter).AddVideoSegment(Array.Empty<byte>());
                        (this.VideoTransmitter as VideoTransmitter).AddVideoSegment(bytes);
                        await Task.Delay(1000 / 30);
                    }
                    else
                    {
                        spinWait.SpinOnce();
                    }
                }
            });
        }

        private async void StateThread()
        {
            await Task.Run(async () =>
            {
                while (true)
                {
                    if (this.inCommandMode)
                    {
                        (this.StateTransmitter as StateTransmitter).SetState(this.state);
                    }

                    await Task.Delay(1000 / 5);
                }
            });
        }

        private string DroneSimulator_CommandReceived(Command command)
        {
            try
            {
                return this.Invoke(command);
            }
            catch (Exception ex)
            {
                return $"error {ex.GetType().Name}: {ex.Message}";
            }
        }

        private string Invoke(Command command)
        {
            if (command != Commands.EnterSdkMode && !this.inCommandMode)
            {
                throw new TelloException("Call EnterSdkMode first.");
            }

            if (!this.isFlying && command.Rule.MustBeInFlight)
            {
                throw new TelloException("Call Takeoff first.");
            }

            switch (command.Rule.Response)
            {
                case Responses.Ok:
                    this.HandleOk(command);
                    this.state = new TelloState(this.position);
                    return "ok";
                case Responses.Speed:
                    return this.speed.ToString();
                case Responses.Battery:
                    return "99";
                case Responses.Time:
                    return "0";
                case Responses.WIFISnr:
                    return "unk";
                case Responses.SdkVersion:
                    return "Sim V1";
                case Responses.SerialNumber:
                    return "SIM-1234";
                case Responses.None:
                    return String.Empty;
                default:
                    throw new NotSupportedException();
            }
        }

        private void HandleOk(Command command)
        {
            switch ((Commands)command)
            {
                case Commands.EnterSdkMode:
                    this.inCommandMode = true;
                    break;

                case Commands.Takeoff:
                    this.height = 20;
                    this.isFlying = true;
                    break;

                case Commands.EmergencyStop:
                case Commands.Land:
                    this.height = 0;
                    this.isFlying = false;
                    break;

                case Commands.StartVideo:
                    this.isVideoStreaming = true;
                    break;
                case Commands.StopVideo:
                    this.isVideoStreaming = false;
                    break;

                case Commands.Left:
                    this.position = this.position.Move(CardinalDirections.Left, (int)command.Arguments[0]);
                    break;
                case Commands.Right:
                    this.position = this.position.Move(CardinalDirections.Right, (int)command.Arguments[0]);
                    break;
                case Commands.Forward:
                    this.position = this.position.Move(CardinalDirections.Front, (int)command.Arguments[0]);
                    break;
                case Commands.Back:
                    this.position = this.position.Move(CardinalDirections.Back, (int)command.Arguments[0]);
                    break;
                case Commands.ClockwiseTurn:
                    this.position = this.position.Turn(ClockDirections.Clockwise, (int)command.Arguments[0]);
                    break;
                case Commands.CounterClockwiseTurn:
                    this.position = this.position.Turn(ClockDirections.CounterClockwise, (int)command.Arguments[0]);
                    break;
                case Commands.Go:
                    this.position = this.position.Go((int)command.Arguments[0], (int)command.Arguments[1]);
                    break;

                case Commands.SetSpeed:
                    this.speed = (int)command.Arguments[0];
                    break;

                case Commands.Stop:
                    break;

                case Commands.Up:
                    this.height += (int)command.Arguments[0];
                    break;
                case Commands.Down:
                    this.height -= (int)command.Arguments[0];
                    if (this.height < 0)
                    {
                        this.height = 0;
                    }

                    break;

                case Commands.Curve:
                    break;
                case Commands.Flip:
                    break;

                case Commands.SetRemoteControl:
                    break;
                case Commands.SetWiFiPassword:
                    break;
                case Commands.SetStationMode:
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
