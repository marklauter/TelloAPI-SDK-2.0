using System.Collections.Generic;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    //todo: add mission padd commands
    internal static class CommandParser
    {
        public static Commands GetCommand(string message)
        {
            var firstSpace = message.IndexOf(' ');
            if (firstSpace > 0)
            {
                message = message.Substring(0, firstSpace);
            }

            return _commands[message];
        }

        public static string[] GetArgs(string message)
        {
            var tokens = message.Split(' ');

            if (tokens.Length < 2)
            {
                return null;
            }

            var result = new string[tokens.Length - 1];

            for (var i = 0; i < tokens.Length - 1; ++i)
            {
                result[i] = tokens[i + 1];
            }

            return result;
        }

        private static Dictionary<string, Commands> _commands { get; } = new Dictionary<string, Commands>()
        {
            { "command", Commands.EnterSdkMode },
            { "takeoff", Commands.Takeoff },
            { "land", Commands.Land },
            { "streamon", Commands.StartVideo },
            { "streamoff", Commands.StopVideo },
            { "emergency", Commands.EmergencyStop },
            { "up", Commands.Up },
            { "down", Commands.Down },
            { "left", Commands.Left },
            { "right", Commands.Right },
            { "forward", Commands.Forward },
            { "back", Commands.Back },
            { "cw", Commands.ClockwiseTurn },
            { "ccw", Commands.CounterClockwiseTurn },
            { "flip", Commands.Flip },
            { "go", Commands.Go },
            { "stop", Commands.Stop },
            { "curve", Commands.Curve },
            { "speed", Commands.SetSpeed },
            { "rc", Commands.SetRemoteControl },
            { "wifi", Commands.SetWiFiPassword },
            //{ "mon", Messages.SetMissionPadOn },
            //{ "moff", Messages.SetMissionPadOff },
            //{ "mdirection", Messages.SetMissionPadDirection },
            { "ap", Commands.SetStationMode },
            { "speed?", Commands.GetSpeed },
            { "battery?", Commands.GetBattery },
            { "time?", Commands.GetTime },
            { "wifi?", Commands.GetWiFiSnr },
            { "sdk?", Commands.GetSdkVersion },
            { "sn?", Commands.GetSerialNumber },
        };
    }
}
