// <copyright file="TelloEmulator.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// namespace Tello.Emulator.SDKV2
// {
//    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

// //todo: add debug output to every call in the system so the user (programmer) can see what's happening (use the Log static class)
//    public class TelloEmulator : IMessengerService
//    {
//        public TelloEmulator()
//        {
//            _droneState = new RawDroneStateEmulated();

// StateServer = new StateServer(_droneState);
//            var videoServer = new VideoServer(30, 0);
//            VideoServer = videoServer;
//            VideoSampleProvider = videoServer;

// _stateManager = new StateManager((RawDroneStateEmulated)_droneState, (VideoServer)VideoServer, (StateServer)StateServer);
//            _commandInterpreter = new CommandInterpreter(_stateManager);
//        }

// public IMessageRelayService<IRawDroneState> StateServer { get; }
//        public IMessageRelayService<IVideoSample> VideoServer { get; }
//        public IVideoSampleProvider VideoSampleProvider { get; }

// public Position Position => _stateManager.Position;

// private readonly IRawDroneState _droneState;
//        private readonly StateManager _stateManager;
//        private readonly CommandInterpreter _commandInterpreter;

// public bool IsPoweredUp => _stateManager.IsPoweredUp;

// public void PowerOn()
//        {
//            if (!IsPoweredUp)
//            {
//                _stateManager.PowerOn();
//                MessengerState = MessengerStates.Disconnected;
//            }
//        }

// public void PowerOff()
//        {
//            _stateManager.PowerOff();
//            MessengerState = MessengerStates.Disconnected;
//        }

// #region IMessengerService

// public MessengerStates MessengerState { get; private set; } = MessengerStates.Disconnected;

// public void Connect()
//        {
//            if (!_stateManager.IsPoweredUp)
//            {
//                throw new TelloUnavailableException("Device must be connected to Tello's WIFI.");
//            }

// if (MessengerState == MessengerStates.Disconnected)
//            {
//                MessengerState = MessengerStates.Connected;
//            }
//        }

// public void Disconnect()
//        {
//            MessengerState = MessengerStates.Disconnected;
//        }

// public Task<IResponse> SendAsync(IMessage request)
//        {
//            if (MessengerState != MessengerStates.Connected)
//            {
//                return Task.FromResult(Response.FromException(request, new InvalidOperationException("Not connected."), TimeSpan.FromSeconds(0)));
//            }

// var clock = Stopwatch.StartNew();
//            var message = Encoding.UTF8.GetString(request.Data);

// var response = _commandInterpreter.Interpret(message);

// return Task.FromResult(Response.FromData(request, Encoding.UTF8.GetBytes(response), clock.Elapsed));
//        }
//        #endregion
//    }
// }
