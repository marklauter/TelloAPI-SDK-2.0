// <copyright file="Command.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Text;

namespace Tello
{
    /// <summary>
    /// https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf.
    /// </summary>
    public sealed class Command
    {
        #region ctor
        public Command()
            : this(Commands.EnterSdkMode)
        {
        }

        public Command(Commands command)
            : this(command, null)
        {
        }

        public Command(Commands command, params object[] args)
        {
            this.Rule = CommandRules.Rules(command);
            this.Validate(command, args);
            this.Arguments = args;
            this.Immediate = this.Rule.Immediate;
        }
        #endregion

        /// <summary>
        /// Gets a value containing the command arguments.
        /// </summary>
        public object[] Arguments { get; }

        /// <summary>
        /// Gets a value indicating whether the command is executed immediately or queued for execution after the current command terminates.
        /// </summary>
        /// <remarks>
        /// Indicates whether or not the flight controller should queue the command (Immediate == false) or send the command immediately (Immediate == true).
        /// Examples of immediate commands include Set4ChannelRC and EmergencyStop.
        /// Examples of non-immediate commands include Move, Land and Flip.
        /// </remarks>
        public bool Immediate { get; }

        /// <summary>
        /// Gets the ruleset that governs the exectution of the command.
        /// </summary>
        public CommandRule Rule { get; }

        #region operators
        public static implicit operator Command(Commands command)
        {
            return new Command(command);
        }

        public static implicit operator Commands(Command command)
        {
            return command.Rule.Command;
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

        public static explicit operator string(Command command)
        {
            return command?.ToString();
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

            var rule = CommandRules.Rules(tokens[0]);

            if (rule.Arguments.Length != tokens.Length - 1)
            {
                throw new ArgumentOutOfRangeException($"{rule.Command}: argument count mismatch. expected: {rule.Arguments.Length} actual: {tokens.Length - 1}");
            }

            if (rule.Arguments.Length == 0)
            {
                return new Command(rule.Command);
            }
            else
            {
                var args = new object[rule.Arguments.Length];
                for (var i = 0; i < rule.Arguments.Length; ++i)
                {
                    args[i] = Convert.ChangeType(tokens[i + 1], rule.Arguments[i].Type);
                }

                return new Command(rule.Command, args);
            }
        }

        public static explicit operator Responses(Command command)
        {
            return command.Rule.Response;
        }

        // todo: move command timeouts to command rules
        public static explicit operator TimeSpan(Command command)
        {
            var avgspeed = 10.0; // cm/s (using a low speed to give margin of error)
            var arcspeed = 15.0; // degrees/s (tested at 30 degress/second, but want to add some margin for error)
            double distance;

            switch (command.Rule.Command)
            {
                case Commands.EnterSdkMode:
                case Commands.EmergencyStop:
                case Commands.GetSpeed:
                case Commands.GetBattery:
                case Commands.GetTime:
                case Commands.GetWIFISnr:
                case Commands.GetSdkVersion:
                case Commands.GetSerialNumber:
                    return TimeSpan.FromSeconds(30);

                // todo: if I knew the set speed in cm/s I could get a better timeout value
                case Commands.Left:
                case Commands.Right:
                case Commands.Forward:
                case Commands.Back:
                    distance = (int)command.Arguments[0]; // cm
                    return TimeSpan.FromSeconds(distance / avgspeed * 10);

                case Commands.Go:
                    var x = (int)command.Arguments[0];
                    var y = (int)command.Arguments[1];
                    distance = Math.Sqrt(Math.Pow(x, 2) + Math.Pow(y, 2));
                    return TimeSpan.FromSeconds(distance / avgspeed * 10);

                case Commands.ClockwiseTurn:
                case Commands.CounterClockwiseTurn:
                    var degrees = (int)command.Arguments[0];
                    return TimeSpan.FromSeconds(degrees / arcspeed * 10);

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
            if (args == null && this.Rule.Arguments.Length > 0)
            {
                throw new ArgumentNullException($"{command}: {nameof(args)}");
            }

            if (args != null && args.Length != this.Rule.Arguments.Length)
            {
                throw new ArgumentException(
                    $"{command}: argument count mismatch. expected: {this.Rule.Arguments.Length} actual: {(args == null ? 0 : args.Length)}");
            }

            if (this.Rule.Arguments.Length > 0)
            {
                for (var i = 0; i < args.Length; ++i)
                {
                    var arg = args[i];
                    if (arg == null)
                    {
                        throw new ArgumentNullException($"{command}: {nameof(args)}[{i}]");
                    }

                    var argumentRule = this.Rule.Arguments[i];
                    if (!argumentRule.IsTypeAllowed(arg))
                    {
                        throw new ArgumentException($"{command}: {nameof(args)}[{i}] type mismatch. expected: '{argumentRule.Type.Name}' actual: '{arg.GetType().Name}'");
                    }

                    if (!argumentRule.IsValueAllowed(Convert.ChangeType(arg, argumentRule.Type)))
                    {
                        throw new ArgumentOutOfRangeException($"{command}: {nameof(args)}[{i}] argument out of range: {arg}");
                    }
                }

                switch (command)
                {
                    case Commands.Go:
                    case Commands.Curve:
                        var twentyCount = 0;
                        for (var i = 0; i < args.Length - 1; ++i)
                        {
                            twentyCount += Math.Abs((int)args[i]) <= 20
                                ? 1
                                : 0;
                            if (twentyCount > 1)
                            {
                                throw new ArgumentOutOfRangeException($"{command}: {nameof(args)} x, y and z can't match /[-20-20]/ simultaneously.");
                            }
                        }

                        break;
                }
            }
        }

        public override string ToString()
        {
            return CommandRules
                .Rules(this.Rule.Command)
                .ToString(this.Arguments);
        }
    }
}