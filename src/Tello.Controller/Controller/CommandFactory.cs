using System;
using Tello.Messaging;

namespace Tello.Controller
{
    /// <summary>
    /// https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf
    /// </summary>
    internal static class CommandFactory
    {
        private static bool IsNumeric(this Type type)
        {
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Byte:
                case TypeCode.Decimal:
                case TypeCode.Double:
                case TypeCode.Int16:
                case TypeCode.Int32:
                case TypeCode.Int64:
                case TypeCode.SByte:
                case TypeCode.Single:
                case TypeCode.UInt16:
                case TypeCode.UInt32:
                case TypeCode.UInt64:
                    return true;
                default:
                    return false;
            }
        }

        private static bool IsString(this Type type)
        {
            var code = Type.GetTypeCode(type);
            switch (code)
            {
                case TypeCode.Char:
                case TypeCode.String:
                    return true;
                default:
                    return false;
            }
        }

        internal static char ToChar(this CardinalDirections direction)
        {
            switch (direction)
            {
                case CardinalDirections.Left:
                    return 'l';
                case CardinalDirections.Right:
                    return 'r';
                case CardinalDirections.Front:
                    return 'f';
                case CardinalDirections.Back:
                    return 'b';
                default:
                    throw new NotSupportedException(direction.ToString());
            }
        }

        /// <summary>
        /// validate the args count, type and value range for a particular command
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        private static void Validate(TelloCommands command, params object[] args)
        {
            var expectedLength = 0;

            switch (command)
            {
                case TelloCommands.EnterSdkMode:
                case TelloCommands.Takeoff:
                case TelloCommands.Land:
                case TelloCommands.Stop:
                case TelloCommands.StartVideo:
                case TelloCommands.StopVideo:
                case TelloCommands.EmergencyStop:
                case TelloCommands.GetSpeed:
                case TelloCommands.GetBattery:
                case TelloCommands.GetTime:
                case TelloCommands.GetWIFISnr:
                case TelloCommands.GetSdkVersion:
                case TelloCommands.GetSerialNumber:
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    break;

                case TelloCommands.Up:
                case TelloCommands.Down:
                case TelloCommands.Left:
                case TelloCommands.Right:
                case TelloCommands.Forward:
                case TelloCommands.Back:
                    expectedLength = 1;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    if (!args[0].GetType().IsNumeric())
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}[0] to be numeric.");
                    }
                    if ((int)args[0] < 20 || (int)args[0] > 500)
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[0] to match /[20-500]/ .");
                    }
                    break;

                case TelloCommands.ClockwiseTurn:
                case TelloCommands.CounterClockwiseTurn:
                    expectedLength = 1;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    if (!args[0].GetType().IsNumeric())
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}[0] to be numeric.");
                    }
                    if ((int)args[0] < 1 || (int)args[0] > 360)
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[0] to match /[1-360]/ .");
                    }
                    break;

                case TelloCommands.Flip:
                    expectedLength = 1;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    if (!args[0].GetType().IsString())
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

                case TelloCommands.Go:
                    expectedLength = 4;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    var twentyCheck = 0;
                    for (var i = 0; i < args.Length; ++i)
                    {
                        if (!args[i].GetType().IsNumeric())
                        {
                            throw new ArgumentException($"{command} expected {nameof(args)}[{i}] to be numeric.");
                        }
                        if (i < 3 && ((int)args[i] < -500 || (int)args[i] > 500))
                        {
                            twentyCheck += (int)args[i] >= -20 || (int)args[i] <= 20
                                ? 1
                                : 0;
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[-500-500]/.");
                        }
                        if (i == 3 && ((int)args[i] < -10 || (int)args[i] > 100))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[10-100]/.");
                        }
                    }
                    if (twentyCheck == 3)
                    {
                        throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)} x, y and z can't be set between -20 and 20 simultaneously.");
                    }
                    break;

                case TelloCommands.Curve:
                    expectedLength = 7;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    for (var i = 0; i < args.Length; ++i)
                    {
                        if (!args[i].GetType().IsNumeric())
                        {
                            throw new ArgumentException($"{command} expected {nameof(args)}[{i}] to be numeric.");
                        }
                        if (i < 6 && ((int)args[i] < -500 || (int)args[i] > 500))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[-500-500]/.");
                        }
                        if (i == 6 && ((int)args[i] < -10 || (int)args[i] > 60))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[10-60]/.");
                        }
                    }
                    break;

                case TelloCommands.SetSpeed:
                    expectedLength = 1;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    {
                        var i = 0;
                        if (!args[i].GetType().IsNumeric())
                        {
                            throw new ArgumentException($"{command} expected {nameof(args)}[{i}] to be numeric.");
                        }
                        if (i < 6 && ((int)args[i] < -10 || (int)args[i] > 100))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[-100-100]/.");
                        }
                    }
                    break;
                case TelloCommands.SetRemoteControl:
                    expectedLength = 4;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    for (var i = 0; i < args.Length; ++i)
                    {
                        if (!args[i].GetType().IsNumeric())
                        {
                            throw new ArgumentException($"{command} expected {nameof(args)}[{i}] to be numeric.");
                        }
                        if (i < 6 && ((int)args[i] < -100 || (int)args[i] > 100))
                        {
                            throw new ArgumentOutOfRangeException($"{command} expected {nameof(args)}[{i}] to match /[-100-100]/.");
                        }
                    }
                    break;
                case TelloCommands.SetWiFiPassword:
                    expectedLength = 2;
                    if (args.Length != expectedLength)
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

                case TelloCommands.SetStationMode:
                    expectedLength = 2;
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    break;

                default:
                    throw new NotSupportedException();
            }

        }

        /// <summary>
        /// convert command enum to string
        /// </summary>
        /// <param name="command"></param>
        /// <param name="args"></param>
        /// <returns></returns>
        internal static string ToMessage(this TelloCommands command, params object[] args)
        {
            Validate(command, args);

            var message = String.Empty;
            switch (command)
            {
                // no args
                case TelloCommands.EnterSdkMode:
                    return "command";
                case TelloCommands.Takeoff:
                    return "takeoff";
                case TelloCommands.Land:
                    return "land";
                case TelloCommands.Stop:
                    return "stop";
                case TelloCommands.StartVideo:
                    return "streamon";
                case TelloCommands.StopVideo:
                    return "streamoff";
                case TelloCommands.EmergencyStop:
                    return "emergency";

                // 1 arg
                case TelloCommands.Up:
                case TelloCommands.Down:
                case TelloCommands.Left:
                case TelloCommands.Right:
                case TelloCommands.Forward:
                case TelloCommands.Back:
                case TelloCommands.Flip:
                    return $"{command.ToString().ToLowerInvariant()} {args[0]}";
                case TelloCommands.ClockwiseTurn:
                    return $"cw {args[0]}";
                case TelloCommands.CounterClockwiseTurn:
                    return $"ccw {args[0]}";

                case TelloCommands.Go:
                case TelloCommands.Curve:
                    message = $"{command.ToString().ToLowerInvariant()}";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case TelloCommands.SetSpeed:
                    message = "speed";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case TelloCommands.SetRemoteControl:
                    message = "rc";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case TelloCommands.SetWiFiPassword:
                    message = "wifi";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                //case Commands.SetMissionPadOn:
                //    return "mon";
                //case Commands.SetMissionPadOff:
                //    return "moff";
                //case Commands.SetMissionPadDirection:
                //    return "mdirection";

                case TelloCommands.SetStationMode:
                    message = "ap";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case TelloCommands.GetSpeed:
                    return "speed?";
                case TelloCommands.GetBattery:
                    return "battery?";
                case TelloCommands.GetTime:
                    return "time?";
                case TelloCommands.GetWIFISnr:
                    return "wifi?";
                case TelloCommands.GetSdkVersion:
                    return "sdk?";
                case TelloCommands.GetSerialNumber:
                    return "sn?";

                default:
                    throw new NotSupportedException();
            }
        }

        internal static TimeSpan ToTimeout(this TelloCommands command, params object[] args)
        {
            Validate(command, args);

            var avgspeed = 10.0; // cm/s (using a low speed to give margin of error)
            var distance = 0.0;
            var arcspeed = 15.0; // degrees/s (tested at 30 degress/second, but want to add some margin for error)

            switch (command)
            {
                case TelloCommands.EnterSdkMode:
                case TelloCommands.EmergencyStop:
                    return TimeSpan.FromSeconds(5);

                case TelloCommands.SetSpeed:
                case TelloCommands.SetRemoteControl:
                case TelloCommands.SetWiFiPassword:
                case TelloCommands.SetStationMode:
                case TelloCommands.StartVideo:
                case TelloCommands.StopVideo:
                    return TimeSpan.FromSeconds(60);

                case TelloCommands.GetSpeed:
                case TelloCommands.GetBattery:
                case TelloCommands.GetTime:
                case TelloCommands.GetWIFISnr:
                case TelloCommands.GetSdkVersion:
                case TelloCommands.GetSerialNumber:
                    return TimeSpan.FromSeconds(5);

                case TelloCommands.Takeoff:
                    return TimeSpan.FromSeconds(20);

                //todo: if I knew the set speed in cm/s I could get a better timeout value
                case TelloCommands.Left:
                case TelloCommands.Right:
                case TelloCommands.Forward:
                case TelloCommands.Back:
                    distance = (int)args[0]; //cm
                    return TimeSpan.FromSeconds(distance / avgspeed);

                case TelloCommands.Go:
                    var x = (int)args[0];
                    var y = (int)args[1];
                    distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                    return TimeSpan.FromSeconds(distance / avgspeed);

                case TelloCommands.ClockwiseTurn:
                case TelloCommands.CounterClockwiseTurn:
                    var degrees = (int)args[0];
                    return TimeSpan.FromSeconds(degrees / arcspeed);

                case TelloCommands.Flip:
                //todo: I don't know how to take the args for this command to generate an arc from which I can determin distance
                case TelloCommands.Curve:
                case TelloCommands.Land:
                case TelloCommands.Stop:
                case TelloCommands.Up:
                case TelloCommands.Down:
                default:
                    return TimeSpan.FromSeconds(60);
            }
        }

        internal static Responses ToReponse(this TelloCommands command)
        {
            switch (command)
            {
                case TelloCommands.GetSpeed:
                    return Responses.Speed;

                case TelloCommands.GetBattery:
                    return Responses.Battery;

                case TelloCommands.GetTime:
                    return Responses.Time;

                case TelloCommands.GetWIFISnr:
                    return Responses.WIFISnr;

                case TelloCommands.GetSdkVersion:
                    return Responses.SdkVersion;

                case TelloCommands.GetSerialNumber:
                    return Responses.SerialNumber;

                default:
                    return Responses.Ok;
            }
        }
    }
}
