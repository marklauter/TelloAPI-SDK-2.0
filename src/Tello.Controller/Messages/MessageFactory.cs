using System;

namespace Tello.Controller.Messages
{
    /// <summary>
    /// https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf
    /// </summary>
    internal static class MessageFactory
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

        internal static char ToChar(this FlipDirections direction)
        {
            switch (direction)
            {
                case FlipDirections.Left:
                    return 'l';
                case FlipDirections.Right:
                    return 'r';
                case FlipDirections.Front:
                    return 'f';
                case FlipDirections.Back:
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
        private static void Validate(Messages command, params object[] args)
        {
            var expectedLength = 0;

            switch (command)
            {
                case Messages.EstablishLink:
                case Messages.Takeoff:
                case Messages.Land:
                case Messages.Stop:
                case Messages.StartVideo:
                case Messages.StopVideo:
                case Messages.EmergencyStop:
                case Messages.GetSpeed:
                case Messages.GetBattery:
                case Messages.GetTime:
                case Messages.GetWiFiSnr:
                case Messages.GetSdkVersion:
                case Messages.GetSerialNumber:
                    if (args.Length != expectedLength)
                    {
                        throw new ArgumentException($"{command} expected {nameof(args)}.Length == {expectedLength}");
                    }
                    break;

                case Messages.Up:
                case Messages.Down:
                case Messages.Left:
                case Messages.Right:
                case Messages.Forward:
                case Messages.Back:
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

                case Messages.ClockwiseTurn:
                case Messages.CounterClockwiseTurn:
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

                case Messages.Flip:
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

                case Messages.Go:
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

                case Messages.Curve:
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

                case Messages.SetSpeed:
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
                case Messages.SetRemoteControl:
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
                case Messages.SetWiFiPassword:
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

                case Messages.SetStationMode:
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
        public static string ToMessage(this Messages command, params object[] args)
        {
            Validate(command, args);

            var message = String.Empty;
            switch (command)
            {
                // no args
                case Messages.EstablishLink:
                    return "command";
                case Messages.Takeoff:
                    return "takeoff";
                case Messages.Land:
                    return "land";
                case Messages.Stop:
                    return "stop";
                case Messages.StartVideo:
                    return "streamon";
                case Messages.StopVideo:
                    return "streamoff";
                case Messages.EmergencyStop:
                    return "emergency";

                // 1 arg
                case Messages.Up:
                case Messages.Down:
                case Messages.Left:
                case Messages.Right:
                case Messages.Forward:
                case Messages.Back:
                case Messages.Flip:
                    return $"{command.ToString().ToLowerInvariant()} {args[0]}";
                case Messages.ClockwiseTurn:
                    return $"cw {args[0]}";
                case Messages.CounterClockwiseTurn:
                    return $"ccw {args[0]}";

                case Messages.Go:
                case Messages.Curve:
                    message = $"{command.ToString().ToLowerInvariant()}";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case Messages.SetSpeed:
                    message = "speed";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case Messages.SetRemoteControl:
                    message = "rc";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case Messages.SetWiFiPassword:
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

                case Messages.SetStationMode:
                    message = "ap";
                    for (var i = 0; i < args.Length; ++i)
                    {
                        message += $" {args[0]}";
                    }

                    return message;

                case Messages.GetSpeed:
                    return "speed?";
                case Messages.GetBattery:
                    return "battery?";
                case Messages.GetTime:
                    return "time?";
                case Messages.GetWiFiSnr:
                    return "wifi?";
                case Messages.GetSdkVersion:
                    return "sdk?";
                case Messages.GetSerialNumber:
                    return "sn?";

                default:
                    throw new NotSupportedException();
            }
        }

        public static TimeSpan ToTimeout(this Messages command, params object[] args)
        {
            Validate(command, args);

            var avgspeed = 60.0; // cm/s
            var distance = 0.0;
            var arcspeed = 45.0; // degrees/s

            switch (command)
            {
                case Messages.SetSpeed:
                case Messages.SetRemoteControl:
                case Messages.SetWiFiPassword:
                case Messages.SetStationMode:
                case Messages.GetSpeed:
                case Messages.GetBattery:
                case Messages.GetTime:
                case Messages.GetWiFiSnr:
                case Messages.GetSdkVersion:
                case Messages.GetSerialNumber:
                case Messages.EstablishLink:
                case Messages.StartVideo:
                case Messages.StopVideo:
                case Messages.EmergencyStop:
                    return TimeSpan.FromSeconds(5);

                case Messages.Takeoff:
                    return TimeSpan.FromSeconds(15);
                case Messages.Land:
                    return TimeSpan.FromSeconds(30);
                case Messages.Stop:
                    return TimeSpan.FromSeconds(10);


                case Messages.Up:
                case Messages.Down:
                    return TimeSpan.FromSeconds(10);

                //todo: if I knew the speed in cm/s I could get a better idea how far
                case Messages.Left:
                case Messages.Right:
                case Messages.Forward:
                case Messages.Back:
                    distance = (int)args[0]; //cm
                    return TimeSpan.FromSeconds(distance / avgspeed);

                case Messages.Go:
                    var x = (int)args[0];
                    var y = (int)args[1];
                    distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                    return TimeSpan.FromSeconds(distance / avgspeed + 10);

                case Messages.ClockwiseTurn:
                case Messages.CounterClockwiseTurn:
                    var degrees = (int)args[0];
                    return TimeSpan.FromSeconds(degrees / arcspeed + 5);

                case Messages.Flip:
                    return TimeSpan.FromSeconds(10);

                //todo: I don't know how to take the args for this command to generate an arc from which I can determin distance
                case Messages.Curve:
                    return TimeSpan.FromSeconds(30);

                default:
                    return TimeSpan.FromSeconds(30);
            }
        }

        internal static Responses ToReponse(this Messages command)
        {
            switch (command)
            {
                case Messages.GetSpeed:
                    return Responses.Speed;

                case Messages.GetBattery:
                    return Responses.Battery;

                case Messages.GetTime:
                    return Responses.Time;

                case Messages.GetWiFiSnr:
                    return Responses.WiFiSnr;

                case Messages.GetSdkVersion:
                    return Responses.SdkVersion;

                case Messages.GetSerialNumber:
                    return Responses.SerialNumber;

                default:
                    return Responses.Ok;
            }
        }
    }
}
