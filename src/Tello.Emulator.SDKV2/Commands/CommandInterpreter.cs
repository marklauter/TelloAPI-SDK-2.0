using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal sealed class CommandInterpreter
    {
        public CommandInterpreter(StateController stateController)
        {
            _stateController = stateController ?? throw new ArgumentNullException(nameof(stateController));
        }

        private readonly StateController _stateController;

        private readonly string _ok = "ok";
        private readonly string _error = "error";

        public async Task<string> InterpretAsync(string message)
        {
            try
            {
                var command = CommandParser.GetCommand(message);
                if (!_stateController.IsSdkModeActivated && command != Commands.EnterSdkMode)
                {
                    Debug.WriteLine($"{nameof(CommandInterpreter)} - not in SDK mode. Message ignored: {message}");
                    return null;
                }
                Debug.WriteLine($"{nameof(CommandInterpreter)} - message received: {message}, command identified: {command}");

                var args = CommandParser.GetArgs(message);
                switch (command)
                {
                    case Commands.EnterSdkMode:
                        if (!_stateController.IsSdkModeActivated)
                        {
                            await _stateController.EnterSdkMode();
                            return _ok;
                        }
                        return null;
                    case Commands.Takeoff:
                        await _stateController.TakeOff();
                        return _ok;
                    case Commands.Land:
                        await _stateController.Land();
                        return _ok;
                    case Commands.StartVideo:
                        _stateController.StartVideo();
                        return _ok;
                    case Commands.StopVideo:
                        _stateController.StopVideo();
                        return _ok;
                    case Commands.Stop:
                        return _ok;
                    case Commands.EmergencyStop:
                        await _stateController.EmergencyStop();
                        return _ok;
                    case Commands.Up:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.GoUp(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Down:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.GoDown(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Left:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.GoLeft(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Right:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.GoRight(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Forward:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.GoForward(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Back:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.GoBack(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.ClockwiseTurn:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.TurnClockwise(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.CounterClockwiseTurn:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _stateController.TurnCounterClockwise(Int32.Parse(args[0]));
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
                        await _stateController.Go(
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
                        await _stateController.SetSpeed(Int32.Parse(args[0]));
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
                        return _stateController.GetSpeed().ToString();
                    case Commands.GetBattery:
                        return _stateController.GetBattery().ToString();
                    case Commands.GetTime:
                        return _stateController.GetTime().ToString();
                    case Commands.GetWiFiSnr:
                        return "snr";
                    case Commands.GetSdkVersion:
                        return "V2.0 (emulated)";
                    case Commands.GetSerialNumber:
                        return "Tello Emulator V0.1";
                    default:
                        return _error;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"{nameof(CommandInterpreter)} - {ex}");
                return _error;
            }
        }
    }
}
