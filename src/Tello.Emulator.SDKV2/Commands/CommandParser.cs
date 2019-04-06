using System.Collections.Generic;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    //todo: add mission padd commands
    internal static class CommandParser
    {
        public static TelloCommands GetCommand(string message)
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

        private static Dictionary<string, TelloCommands> _commands { get; } = new Dictionary<string, TelloCommands>()
        {
            { "command", TelloCommands.EnterSdkMode },
            { "takeoff", TelloCommands.Takeoff },
            { "land", TelloCommands.Land },
            { "streamon", TelloCommands.StartVideo },
            { "streamoff", TelloCommands.StopVideo },
            { "emergency", TelloCommands.EmergencyStop },
            { "up", TelloCommands.Up },
            { "down", TelloCommands.Down },
            { "left", TelloCommands.Left },
            { "right", TelloCommands.Right },
            { "forward", TelloCommands.Forward },
            { "back", TelloCommands.Back },
            { "cw", TelloCommands.ClockwiseTurn },
            { "ccw", TelloCommands.CounterClockwiseTurn },
            { "flip", TelloCommands.Flip },
            { "go", TelloCommands.Go },
            { "stop", TelloCommands.Stop },
            { "curve", TelloCommands.Curve },
            { "speed", TelloCommands.SetSpeed },
            { "rc", TelloCommands.SetRemoteControl },
            { "wifi", TelloCommands.SetWiFiPassword },
            //{ "mon", Messages.SetMissionPadOn },
            //{ "moff", Messages.SetMissionPadOff },
            //{ "mdirection", Messages.SetMissionPadDirection },
            { "ap", TelloCommands.SetStationMode },
            { "speed?", TelloCommands.GetSpeed },
            { "battery?", TelloCommands.GetBattery },
            { "time?", TelloCommands.GetTime },
            { "wifi?", TelloCommands.GetWIFISnr },
            { "sdk?", TelloCommands.GetSdkVersion },
            { "sn?", TelloCommands.GetSerialNumber },
        };
    }
}
