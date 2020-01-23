// <copyright file="CommandInterpreter.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>
// namespace Tello.Emulator.SDKV2
// {
//    internal sealed class CommandInterpreter
//    {
//        public CommandInterpreter(StateManager stateManager)
//        {
//            _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
//        }

// private readonly StateManager _stateManager;
//        private readonly string _ok = "ok";
//        private readonly string _error = "error";

// public string Interpret(string message)
//        {
//            try
//            {
//                var command = CommandParser.GetCommand(message);
//                if (!_stateManager.IsPoweredUp)
//                {
//                    Log.WriteLine($"not powered up. Message ignored: {message}");
//                    return null;
//                }
//                if (!_stateManager.IsSdkModeActivated && command != TelloCommands.EnterSdkMode)
//                {
//                    Log.WriteLine($"not in SDK mode. Message ignored: {message}");
//                    return null;
//                }
//                Log.WriteLine($"message received: {message}, command identified: {command}");

// var args = CommandParser.GetArgs(message);
//                switch (command)
//                {
//                    case TelloCommands.EnterSdkMode:
//                        if (!_stateManager.IsSdkModeActivated)
//                        {
//                            _stateManager.EnterSdkMode();
//                            if (_stateManager.IsSdkModeActivated)
//                            {
//                                return _ok;
//                            }
//                            else
//                            {
//                                return _error;
//                            }
//                        }
//                        return null;
//                    case TelloCommands.Takeoff:
//                        _stateManager.TakeOff();
//                        return _ok;
//                    case TelloCommands.Land:
//                        _stateManager.Land();
//                        return _ok;
//                    case TelloCommands.StartVideo:
//                        _stateManager.StartVideo();
//                        return _ok;
//                    case TelloCommands.StopVideo:
//                        _stateManager.StopVideo();
//                        return _ok;
//                    case TelloCommands.Stop:
//                        return _ok;
//                    case TelloCommands.EmergencyStop:
//                        _stateManager.EmergencyStop();
//                        return _ok;
//                    case TelloCommands.Up:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.GoUp(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.Down:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.GoDown(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.Left:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.GoLeft(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.Right:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.GoRight(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.Forward:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.GoForward(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.Back:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.GoBack(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.ClockwiseTurn:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.TurnClockwise(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.CounterClockwiseTurn:
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.TurnCounterClockwise(Int32.Parse(args[0]));
//                        return _ok;
//                    case TelloCommands.Flip:
//                    {
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        // there's no state to manage for zero sum movements
//                        return _ok;
//                    }
//                    case TelloCommands.Go:
//                    {
//                        if (args.Length != 4)
//                        {
//                            return _error;
//                        }
//                        _stateManager.Go(
//                            Int32.Parse(args[0]),
//                            Int32.Parse(args[1]),
//                            Int32.Parse(args[2]),
//                            Int32.Parse(args[3]));
//                        return _ok;
//                    }
//                    case TelloCommands.Curve:
//                    {
//                        if (args.Length != 7)
//                        {
//                            return _error;
//                        }
//                        //todo: implement curve in _flightController
//                        return _ok;
//                    }
//                    case TelloCommands.SetSpeed:
//                    {
//                        if (args.Length != 1)
//                        {
//                            return _error;
//                        }
//                        _stateManager.SetSpeed(Int32.Parse(args[0]));
//                        return _ok;
//                    }
//                    case TelloCommands.SetRemoteControl:
//                    {
//                        if (args.Length != 4)
//                        {
//                            return _error;
//                        }
//                        if (Int32.Parse(args[0]) < -100 || Int32.Parse(args[0]) > 100
//                            || Int32.Parse(args[1]) < -100 || Int32.Parse(args[1]) > 100
//                            || Int32.Parse(args[2]) < -100 || Int32.Parse(args[2]) > 100
//                            || Int32.Parse(args[3]) < -100 || Int32.Parse(args[3]) > 100)
//                        {
//                            return _error;
//                        }
//                        return _ok;
//                    }
//                    case TelloCommands.SetWiFiPassword:
//                    {
//                        if (args.Length != 2)
//                        {
//                            return _error;
//                        }
//                        return _ok;
//                    }
//                    #region mission pad
//                    //case Commands.SetMissionPadOn:
//                    //    //if (_droneState.MissionPadDected == -1)
//                    //    //{
//                    //    //    return _error;
//                    //    //}
//                    //    _inMissionPadMode = true;
//                    //    return _ok;
//                    //case Commands.SetMissionPadOff:
//                    //    _inMissionPadMode = false;
//                    //    return _ok;
//                    //case Commands.SetMissionPadDirection:
//                    //    if (!_inMissionPadMode)
//                    //    {
//                    //        return null;
//                    //    }
//                    //    else
//                    //    {
//                    //        var args = CommandParser.GetArgs(message);
//                    //        if (args.Length != 1)
//                    //        {
//                    //            return _error;
//                    //        }
//                    //        var direction = Int32.Parse(args[0]);
//                    //        if (direction < 0 || direction > 2)
//                    //        {
//                    //            return _error;
//                    //        }
//                    //        return _ok;
//                    //    }
//                    #endregion
//                    case TelloCommands.SetStationMode:
//                    {
//                        if (args.Length != 2)
//                        {
//                            return _error;
//                        }
//                        return _ok;
//                    }
//                    case TelloCommands.GetSpeed:
//                    {
//                        var value = _stateManager.GetSpeed();
//                        return value != -1
//                            ? value.ToString()
//                            : null;
//                    }
//                    case TelloCommands.GetBattery:
//                    {
//                        var value = _stateManager.GetBattery();
//                        return value != -1
//                            ? value.ToString()
//                            : null;
//                    }
//                    case TelloCommands.GetTime:
//                    {
//                        var value = _stateManager.GetTime();
//                        return value != -1
//                            ? value.ToString()
//                            : null;
//                    }
//                    case TelloCommands.GetWIFISnr:
//                        return _stateManager.IsPoweredUp
//                            ? "snr"
//                            : null;
//                    case TelloCommands.GetSdkVersion:
//                        return _stateManager.IsPoweredUp
//                            ? "SDK 2.0-E"
//                            : null;
//                    case TelloCommands.GetSerialNumber:
//                        return _stateManager.IsPoweredUp
//                            ? "Tello SDK 2.0 Emulator"
//                            : null;
//                    default:
//                        return _error;
//                }
//            }
//            catch (Exception ex)
//            {
//                Log.WriteLine($"{nameof(CommandInterpreter)} - {ex}");
//                return _stateManager.IsPoweredUp
//                    ? _error
//                    : null;
//            }
//        }
//    }
// }
