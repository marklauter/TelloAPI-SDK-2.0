using System;
using System.Text;
using Tello.Types;

namespace Tello
{
    /// <summary>
    /// https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf
    /// </summary>
    public sealed class Command
    {
        #region ctor
        public Command()
            : this(Commands.EnterSdkMode) { }

        public Command(Commands command)
            : this(command, null)
        {
        }

        public Command(Commands command, params object[] args)
        {
            Validate(command, args);
            Value = command;
            Arguments = args;
        }
        #endregion

        public Commands Value { get; }
        public object[] Arguments { get; }

        #region operators
        public static implicit operator Command(Commands command)
        {
            return new Command(command);
        }

        public static implicit operator Commands(Command command)
        {
            return command.Value;
        }

        public static explicit operator Command(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException(nameof(bytes));
            }

            if (bytes.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(bytes));
            }

            return (Command)Encoding.UTF8.GetString(bytes);
        }

        public static explicit operator Command(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                throw new ArgumentNullException(nameof(value));
            }

            if (value.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            var tokens = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (tokens.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(value));
            }

            switch (tokens[0])
            {
                case "command":
                    if (tokens.Length > 1)
                    {
                        throw new ArgumentOutOfRangeException("no argumenets expected");
                    }
                    return Commands.EnterSdkMode;
                case "takeoff":
                    if (tokens.Length > 1)
                    {
                        throw new ArgumentOutOfRangeException("no argumenets expected");
                    }
                    return Commands.Takeoff;
                case "land":
                    if (tokens.Length > 1)
                    {
                        throw new ArgumentOutOfRangeException("no argumenets expected");
                    }
                    return Commands.Land;
                case "streamon":
                    if (tokens.Length > 1)
                    {
                        throw new ArgumentOutOfRangeException("no argumenets expected");
                    }
                    return Commands.StartVideo;
                case "streamoff":
                    if (tokens.Length > 1)
                    {
                        throw new ArgumentOutOfRangeException("no argumenets expected");
                    }
                    return Commands.Stop;
                case "emergency":
                    if (tokens.Length > 1)
                    {
                        throw new ArgumentOutOfRangeException("no argumenets expected");
                    }
                    return Commands.EmergencyStop;
                case "up":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.Up, Int32.Parse(tokens[1]));
                case "down":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.Down, Int32.Parse(tokens[1]));
                case "left":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.Left, Int32.Parse(tokens[1]));
                case "right":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.Right, Int32.Parse(tokens[1]));
                case "forward":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.Forward, Int32.Parse(tokens[1]));
                case "back":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.Back, Int32.Parse(tokens[1]));
                case "flip":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.Flip, (CardinalDirection)tokens[1][0]);
                case "cw":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.ClockwiseTurn, Int32.Parse(tokens[1]));
                case "ccw":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.CounterClockwiseTurn, Int32.Parse(tokens[1]));
                case "go":
                    if (tokens.Length != 5)
                    {
                        throw new ArgumentOutOfRangeException("four arguments expected");
                    }
                    return new Command(Commands.Go,
                        Int32.Parse(tokens[1]),
                        Int32.Parse(tokens[2]),
                        Int32.Parse(tokens[3]),
                        Int32.Parse(tokens[4]));
                case "curve":
                    if (tokens.Length != 8)
                    {
                        throw new ArgumentOutOfRangeException("seven arguments expected");
                    }
                    return new Command(Commands.Curve,
                        Int32.Parse(tokens[1]),
                        Int32.Parse(tokens[2]),
                        Int32.Parse(tokens[3]),
                        Int32.Parse(tokens[4]),
                        Int32.Parse(tokens[5]),
                        Int32.Parse(tokens[6]),
                        Int32.Parse(tokens[7]));
                case "speed":
                    if (tokens.Length != 2)
                    {
                        throw new ArgumentOutOfRangeException("one argument expected");
                    }
                    return new Command(Commands.SetSpeed, Int32.Parse(tokens[1]));
                case "rc":
                    if (tokens.Length != 5)
                    {
                        throw new ArgumentOutOfRangeException("four arguments expected");
                    }
                    return new Command(Commands.SetRemoteControl,
                        Int32.Parse(tokens[1]),
                        Int32.Parse(tokens[2]),
                        Int32.Parse(tokens[3]),
                        Int32.Parse(tokens[4]));
                case "wifi":
                    if (tokens.Length != 3)
                    {
                        throw new ArgumentOutOfRangeException("two arguments expected");
                    }
                    return new Command(Commands.SetWiFiPassword,
                        tokens[1],
                        tokens[2]);
                case "ap":
                    if (tokens.Length != 3)
                    {
                        throw new ArgumentOutOfRangeException("two arguments expected");
                    }
                    return new Command(Commands.SetStationMode,
                        tokens[1],
                        tokens[2]);
                case "speed?":
                    return Commands.GetSpeed;
                case "battery?":
                    return Commands.GetBattery;
                case "time?":
                    return Commands.GetTime;
                case "wifi?":
                    return Commands.GetWIFISnr;
                case "sdk?":
                    return Commands.GetSdkVersion;
                case "sn?":
                    return Commands.GetSerialNumber;
                default:
                    throw new NotSupportedException(value);
            }
        }

        public static explicit operator Responses(Command command)
        {
            switch (command.Value)
            {
                case Commands.GetSpeed:
                    return Responses.Speed;
                case Commands.GetBattery:
                    return Responses.Battery;
                case Commands.GetTime:
                    return Responses.Time;
                case Commands.GetWIFISnr:
                    return Responses.WIFISnr;
                case Commands.GetSdkVersion:
                    return Responses.SdkVersion;
                case Commands.GetSerialNumber:
                    return Responses.SerialNumber;
                default:
                    return Responses.Ok;
            }
        }

        public static explicit operator TimeSpan(Command command)
        {
            var avgspeed = 10.0; // cm/s (using a low speed to give margin of error)
            var arcspeed = 15.0; // degrees/s (tested at 30 degress/second, but want to add some margin for error)
            double distance;

            switch (command.Value)
            {
                case Commands.EnterSdkMode:
                case Commands.EmergencyStop:
                case Commands.GetSpeed:
                case Commands.GetBattery:
                case Commands.GetTime:
                case Commands.GetWIFISnr:
                case Commands.GetSdkVersion:
                case Commands.GetSerialNumber:
                    return TimeSpan.FromSeconds(5);

                //todo: if I knew the set speed in cm/s I could get a better timeout value
                case Commands.Left:
                case Commands.Right:
                case Commands.Forward:
                case Commands.Back:
                    distance = (int)command.Arguments[0]; //cm
                    return TimeSpan.FromSeconds(distance / avgspeed);

                case Commands.Go:
                    var x = (int)command.Arguments[0];
                    var y = (int)command.Arguments[1];
                    distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                    return TimeSpan.FromSeconds(distance / avgspeed);

                case Commands.ClockwiseTurn:
                case Commands.CounterClockwiseTurn:
                    var degrees = (int)command.Arguments[0];
                    return TimeSpan.FromSeconds(degrees / arcspeed);

                case Commands.Takeoff:
                case Commands.SetSpeed:
                case Commands.SetRemoteControl:
                case Commands.SetWiFiPassword:
                case Commands.SetStationMode:
                case Commands.StartVideo:
                case Commands.StopVideo:
                case Commands.Flip:
                case Commands.Curve:
                case Commands.Land:
                case Commands.Stop:
                case Commands.Up:
                case Commands.Down:
                default:
                    return TimeSpan.FromSeconds(60);
            }
        }
        #endregion

        private void Validate(Commands command, params object[] args)
        {
            bool InRange(int value, int min, int max) => value >= min || value <= max;

            int expectedLength;

            switch (command)
            {
                case Commands.EnterSdkMode:
                case Commands.Takeoff:
                case Commands.Land:
                case Commands.Stop:
                case Commands.StartVideo:
                case Commands.StopVideo:
                case Commands.EmergencyStop:
                case Commands.GetSpeed:
                case Commands.GetBattery:
                case Commands.GetTime:
                case Commands.GetWIFISnr:
                case Commands.GetSdkVersion:
                case Commands.GetSerialNumber:
                    if (args != null)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)} == null");
                    }
                    break;

                case Commands.Up:
                case Commands.Down:
                case Commands.Left:
                case Commands.Right:
                case Commands.Forward:
                case Commands.Back:
                    expectedLength = 1;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    if (!args[0].IsNumeric())
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}[0] to be numeric.");
                    }
                    if (!InRange((int)args[0], 20, 500))
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[0] to match /[20-500]/ .");
                    }
                    break;

                case Commands.ClockwiseTurn:
                case Commands.CounterClockwiseTurn:
                    expectedLength = 1;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    if (!args[0].IsNumeric())
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}[0] to be numeric.");
                    }
                    if (!InRange((int)args[0], 1, 360))
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[0] to match /[1-360]/ .");
                    }
                    break;

                case Commands.Flip:
                    expectedLength = 1;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    if (!args[0].IsString())
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}[0] to be character.");
                    }
                    if (args[0].ToString().Length != 1)
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[0] to be single character.");
                    }
                    if (!"lrfb".Contains(args[0].ToString().ToLowerInvariant()))
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[0] to match /[lrfb]/.");
                    }
                    break;

                case Commands.Go:
                    expectedLength = 4;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    var twentyCheck = 0;
                    for (var i = 0; i < args.Length; ++i)
                    {
                        if (!args[i].IsNumeric())
                        {
                            throw new ArgumentException($"{command} expected {nameof(args)}[{i}] to be numeric.");
                        }
                        if (i < 3)
                        {
                            if (!InRange((int)args[i], -500, 500))
                            {
                                throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[-500-500]/.");
                            }
                            else
                            {
                                twentyCheck += InRange((int)args[i], -20, 20)
                                    ? 1
                                    : 0;
                            }
                        }
                        if (i == 3 && !InRange((int)args[i], 10, 100))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[10-100]/.");
                        }
                    }
                    if (twentyCheck == 3)
                    {
                        throw new ArgumentOutOfRangeException($"{command} {nameof(args)} x, y and z can't match /[-20-20]/ simultaneously.");
                    }
                    break;

                case Commands.Curve:
                    expectedLength = 7;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    for (var i = 0; i < args.Length; ++i)
                    {
                        if (!args[i].IsNumeric())
                        {
                            throw new ArgumentException($"{command} expected {nameof(args)}[{i}] to be numeric.");
                        }
                        if (i < 6 && !InRange((int)args[i], -500, 500))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[-500-500]/.");
                        }
                        if (i == 6 && !InRange((int)args[i], 10, 60))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[10-60]/.");
                        }
                    }
                    break;

                case Commands.SetSpeed:
                    expectedLength = 1;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    if (!args[0].IsNumeric())
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}[0] to be numeric.");
                    }
                    if (!InRange((int)args[0], 10, 100))
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[0] to match /[10-100]/.");
                    }
                    break;

                case Commands.SetRemoteControl:
                    expectedLength = 4;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    for (var i = 0; i < args.Length; ++i)
                    {
                        if (!args[i].IsNumeric())
                        {
                            throw new ArgumentException($"{command} expected {nameof(args)}[{i}] to be numeric.");
                        }
                        if (i < 6 && !InRange((int)args[i], -100, 100))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[-100-100]/.");
                        }
                    }
                    break;

                case Commands.SetWiFiPassword:
                    expectedLength = 2;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    break;

                //case Commands.SetMissionPadOn:
                //    break;
                //case Commands.SetMissionPadOff:
                //    break;
                //case Commands.SetMissionPadDirection:
                //    break;

                case Commands.SetStationMode:
                    expectedLength = 2;
                    if (args == null || args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }
        }

        public override string ToString()
        {
            string message;
            switch (Value)
            {
                // no args
                case Commands.EnterSdkMode:
                    return "command";
                case Commands.Takeoff:
                    return Value.ToString().ToLowerInvariant();
                case Commands.Land:
                    return Value.ToString().ToLowerInvariant();
                case Commands.Stop:
                    return Value.ToString().ToLowerInvariant();
                case Commands.StartVideo:
                    return "streamon";
                case Commands.StopVideo:
                    return "streamoff";
                case Commands.EmergencyStop:
                    return "emergency";

                // 1 arg
                case Commands.Up:
                case Commands.Down:
                case Commands.Left:
                case Commands.Right:
                case Commands.Forward:
                case Commands.Back:
                case Commands.Flip:
                    return $"{Value.ToString().ToLowerInvariant()} {Arguments[0]}";
                case Commands.ClockwiseTurn:
                    return $"cw {Arguments[0]}";
                case Commands.CounterClockwiseTurn:
                    return $"ccw {Arguments[0]}";

                case Commands.Go:
                case Commands.Curve:
                    message = $"{Value.ToString().ToLowerInvariant()}";
                    for (var i = 0; i < Arguments.Length; ++i)
                    {
                        message += $" {Arguments[0]}";
                    }

                    return message;

                case Commands.SetSpeed:
                    message = "speed";
                    for (var i = 0; i < Arguments.Length; ++i)
                    {
                        message += $" {Arguments[0]}";
                    }

                    return message;

                case Commands.SetRemoteControl:
                    message = "rc";
                    for (var i = 0; i < Arguments.Length; ++i)
                    {
                        message += $" {Arguments[0]}";
                    }

                    return message;

                case Commands.SetWiFiPassword:
                    message = "wifi";
                    for (var i = 0; i < Arguments.Length; ++i)
                    {
                        message += $" {Arguments[0]}";
                    }

                    return message;

                //case Commands.SetMissionPadOn:
                //    return "mon";
                //case Commands.SetMissionPadOff:
                //    return "moff";
                //case Commands.SetMissionPadDirection:
                //    return "mdirection";

                case Commands.SetStationMode:
                    message = "ap";
                    for (var i = 0; i < Arguments.Length; ++i)
                    {
                        message += $" {Arguments[0]}";
                    }

                    return message;

                case Commands.GetSpeed:
                    return "speed?";
                case Commands.GetBattery:
                    return "battery?";
                case Commands.GetTime:
                    return "time?";
                case Commands.GetWIFISnr:
                    return "wifi?";
                case Commands.GetSdkVersion:
                    return "sdk?";
                case Commands.GetSerialNumber:
                    return "sn?";

                default:
                    throw new NotSupportedException();
            }
        }
    }
}

