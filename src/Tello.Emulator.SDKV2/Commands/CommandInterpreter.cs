using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal sealed class CommandInterpreter
    {
        public CommandInterpreter(StateManager stateManager)
        {
            _stateManager = stateManager ?? throw new ArgumentNullException(nameof(stateManager));
        }

        private readonly StateManager _stateManager;
        private readonly string _ok = "ok";
        private readonly string _error = "error";

        public async Task<string> InterpretAsync(string message)
        {
            try
            {
                var command = CommandParser.GetCommand(message);
                if (!_stateManager.IsPoweredUp)
                {
                    Log.WriteLine($"not powered up. Message ignored: {message}");
                    return null;
                }
                if (!_stateManager.IsSdkModeActivated && command != Commands.EnterSdkMode)
                {
                    Log.WriteLine($"not in SDK mode. Message ignored: {message}");
                    return null;
                }
                Log.WriteLine($"message received: {message}, command identified: {command}");

                var args = CommandParser.GetArgs(message);
                switch (command)
                {
                    case Commands.EnterSdkMode:
                        if (!_stateManager.IsSdkModeActivated)
                        {
                            await _stateManager.EnterSdkMode();
                            if (_stateManager.IsSdkModeActivated)
                            {
                                return _ok;
                            }
                            else
                            {
                                return _error;
                            }
                        }
                        return null;
                    case Commands.Takeoff:
                        await _stateManager.TakeOff();
                        return _ok;
                    case Commands.Land:
                        await _stateManager.Land();
                        return _ok;
                    case Commands.StartVideo:
                        await _stateManager.StartVideo();
                        return _ok;
                    case Commands.StopVideo:
                        await _stateManager.StopVideo();
                        return _ok;
                    case Commands.Stop:
                        return _ok;
                    case Commands.EmergencyStop:
                        await _stateManager.EmergencyStop();
                        return _ok;
                    case Commands.Up:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.GoUp(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Down:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.GoDown(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Left:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.GoLeft(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Right:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.GoRight(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Forward:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.GoForward(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Back:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.GoBack(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.ClockwiseTurn:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.TurnClockwise(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.CounterClockwiseTurn:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.TurnCounterClockwise(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Flip:
                    {
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        // there's no state to manage for zero sum movements
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        return _ok;
                    }
                    case Commands.Go:
                    {
                        if (args.Length != 4)
                        {
                            return _error;
                        }
                        await _stateManager.Go(
                            Int32.Parse(args[0]),
                            Int32.Parse(args[1]),
                            Int32.Parse(args[2]),
                            Int32.Parse(args[3]));
                        return _ok;
                    }
                    case Commands.Curve:
                    {
                        if (args.Length != 7)
                        {
                            return _error;
                        }
                        //todo: implement curve in _flightController
                        await Task.Delay(TimeSpan.FromSeconds(1));
                        return _ok;
                    }
                    case Commands.SetSpeed:
                    {
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateManager.SetSpeed(Int32.Parse(args[0]));
                        return _ok;
                    }
                    case Commands.SetRemoteControl:
                    {
                        if (args.Length != 4)
                        {
                            return _error;
                        }
                        if (Int32.Parse(args[0]) < -100 || Int32.Parse(args[0]) > 100
                            || Int32.Parse(args[1]) < -100 || Int32.Parse(args[1]) > 100
                            || Int32.Parse(args[2]) < -100 || Int32.Parse(args[2]) > 100
                            || Int32.Parse(args[3]) < -100 || Int32.Parse(args[3]) > 100)
                        {
                            return _error;
                        }
                        return _ok;
                    }
                    case Commands.SetWiFiPassword:
                    {
                        if (args.Length != 2)
                        {
                            return _error;
                        }
                        return _ok;
                    }
                    #region mission pad
                    //case Commands.SetMissionPadOn:
                    //    //if (_droneState.MissionPadDected == -1)
                    //    //{
                    //    //    return _error;
                    //    //}
                    //    _inMissionPadMode = true;
                    //    return _ok;
                    //case Commands.SetMissionPadOff:
                    //    _inMissionPadMode = false;
                    //    return _ok;
                    //case Commands.SetMissionPadDirection:
                    //    if (!_inMissionPadMode)
                    //    {
                    //        return null;
                    //    }
                    //    else
                    //    {
                    //        var args = CommandParser.GetArgs(message);
                    //        if (args.Length != 1)
                    //        {
                    //            return _error;
                    //        }
                    //        var direction = Int32.Parse(args[0]);
                    //        if (direction < 0 || direction > 2)
                    //        {
                    //            return _error;
                    //        }
                    //        return _ok;
                    //    }
                    #endregion
                    case Commands.SetStationMode:
                    {
                        if (args.Length != 2)
                        {
                            return _error;
                        }
                        return _ok;
                    }
                    case Commands.GetSpeed:
                    {
                        var value = await _stateManager.GetSpeed();
                        return value != -1
                            ? value.ToString()
                            : null;
                    }
                    case Commands.GetBattery:
                    {
                        var value = await _stateManager.GetBattery();
                        return value != -1
                            ? value.ToString()
                            : null;
                    }
                    case Commands.GetTime:
                    {
                        var value = await _stateManager.GetTime();
                        return value != -1
                            ? value.ToString()
                            : null;
                    }
                    case Commands.GetWiFiSnr:
                        return _stateManager.IsPoweredUp
                            ? "snr"
                            : null;
                    case Commands.GetSdkVersion:
                        return _stateManager.IsPoweredUp
                            ? "SDK 2.0-E"
                            : null;
                    case Commands.GetSerialNumber:
                        return _stateManager.IsPoweredUp
                            ? "Tello SDK 2.0 Emulator"
                            : null;
                    default:
                        return _error;
                }
            }
            catch (Exception ex)
            {
                Log.WriteLine($"{nameof(CommandInterpreter)} - {ex}");
                return _stateManager.IsPoweredUp
                    ? _error
                    : null;
            }
        }
    }
}
