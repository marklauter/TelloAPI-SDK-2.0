using System;
using System.Threading.Tasks;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    internal class CommandInterpreter
    {
        public CommandInterpreter(FlightController flightController, ILog log)
        {
            _flightController = flightController ?? throw new ArgumentNullException(nameof(flightController));
            _log = log;
        }

        private readonly ILog _log;
        private readonly FlightController _flightController;

        private readonly string _ok = "ok";
        private readonly string _error = "error";

        public async Task<string> InterpretAsync(string message)
        {
            try
            {
                var command = CommandParser.GetCommand(message);
                if (!_flightController.IsSdkModeActivated && command != Commands.EnterSdkMode)
                {
                    Log($"{nameof(CommandInterpreter)} - not in SDK mode. Message ignored: {message}");
                    return null;
                }
                Log($"{nameof(CommandInterpreter)} - message received: {message}, command identified: {command}");

                var args = CommandParser.GetArgs(message);
                switch (command)
                {
                    case Commands.EnterSdkMode:
                        if (!_flightController.IsSdkModeActivated)
                        {
                            await _flightController.EnterSdkMode();
                            return _ok;
                        }
                        return null;
                    case Commands.Takeoff:
                        await _flightController.TakeOff();
                        return _ok;
                    case Commands.Land:
                        await _flightController.Land();
                        return _ok;
                    case Commands.StartVideo:
                        _flightController.StartVideo();
                        return _ok;
                    case Commands.StopVideo:
                        _flightController.StopVideo();
                        return _ok;
                    case Commands.Stop:
                        return _ok;
                    case Commands.EmergencyStop:
                        await _flightController.EmergencyStop();
                        return _ok;
                    case Commands.Up:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.GoUp(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Down:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.GoDown(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Left:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.GoLeft(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Right:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.GoRight(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Forward:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.GoForward(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.Back:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.GoBack(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.ClockwiseTurn:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.TurnClockwise(Int32.Parse(args[0]));
                        return _ok;
                    case Commands.CounterClockwiseTurn:
                        if (args.Length != 1)
                        {
                            return _error;
                        }
                        await _flightController.TurnCounterClockwise(Int32.Parse(args[0]));
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
                        await _flightController.Go(
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
                        await _flightController.SetSpeed(Int32.Parse(args[0]));
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
                        return _flightController.GetSpeed().ToString();
                    case Commands.GetBattery:
                        return _flightController.GetBattery().ToString();
                    case Commands.GetTime:
                        return _flightController.GetTime().ToString();
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
                Log($"{nameof(CommandInterpreter)} - {ex}");
                return _error;
            }
        }

        public void Log(string meesage)
        {
            if (_log != null)
            {
                _log.WriteLine(meesage);
            }
        }
    }
}
