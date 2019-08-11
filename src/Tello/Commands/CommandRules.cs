using System;
using System.Collections.Generic;
using System.Linq;
using Tello.Types;

namespace Tello
{
    // ------------ error messages I've seen -------------
    // error
    // error Motor Stop
    // error Not Joystick

    public abstract class ArgumentRule
    {
        protected ArgumentRule(Type type, Func<object, bool> isTypeAllowed)
        {
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsTypeAllowed = isTypeAllowed ?? throw new ArgumentNullException(nameof(isTypeAllowed));
        }

        public readonly Type Type;
        public readonly Func<object, bool> IsTypeAllowed;

        public abstract bool IsValueAllowed(object value);
    }

    public abstract class ArgumentRule<T> : ArgumentRule
    {
        internal ArgumentRule(Func<object, bool> isTypeAllowed)
            : base(typeof(T), isTypeAllowed)
        {
        }
    }

    public sealed class IntegerRule : ArgumentRule<int>
    {
        public sealed class Range<T>
            where T : IComparable
        {
            internal Range(T min, T max)
            {
                Min = min;
                Max = max;
            }

            public readonly T Min;
            public readonly T Max;

            public bool Contains(T value)
            {
                return value.CompareTo(Min) >= 0 && value.CompareTo(Max) <= 0;
            }
        }

        private readonly Range<int> _range;

        internal IntegerRule(Range<int> range)
            : base(Primitive.IsNumeric)
        {
            _range = range;
        }

        public override bool IsValueAllowed(object value)
        {
            return value is int && _range.Contains((int)value);
        }
    }

    public sealed class CharacterRule : ArgumentRule<char>
    {
        private readonly string _allowedValues;

        internal CharacterRule(string allowedValues)
            : base(Primitive.IsString)
        {
            _allowedValues = allowedValues;
        }

        public override bool IsValueAllowed(object value)
        {
            return (value is char)
                && _allowedValues.Contains(value.ToString());
        }
    }

    public sealed class StringRule : ArgumentRule<string>
    {
        internal StringRule()
            : base(Primitive.IsString)
        {
        }

        public override bool IsValueAllowed(object value)
        {
            return value != null
                && ((string)value).Length > 0;
        }
    }

    public sealed class CommandRule
    {
        internal CommandRule(
            Commands command,
            Responses response,
            string token,
            ArgumentRule[] arguments,
            bool mustBeInflight,
            bool immediate = false)
        {
            Command = command;
            Response = response;
            Token = token;
            Arguments = arguments;
            MustBeInFlight = mustBeInflight;
            Immediate = immediate;
        }

        public readonly Commands Command;
        public readonly Responses Response;
        public readonly string Token;
        public readonly ArgumentRule[] Arguments;
        public readonly bool MustBeInFlight;
        public readonly bool Immediate;

        public string ToString(params object[] args)
        {
            var result = Token;

            if (args != null)
            {
                foreach (var arg in args)
                {
                    result += $" {arg}";
                }
            }

            return result;
        }
    }

    internal static class CommandRules
    {
        static CommandRules()
        {
            var emptyArgs = new ArgumentRule[0];

            var movementArgs = new ArgumentRule[]
            {
                new IntegerRule(new IntegerRule.Range<int>(20, 500))
            };

            var turnArgs = new ArgumentRule[]
            {
                new IntegerRule(new IntegerRule.Range<int>(1, 360))
            };

            _rulesByCommand = new Dictionary<Commands, CommandRule>()
            {
                { Commands.EnterSdkMode, new CommandRule(Commands.EnterSdkMode, Responses.Ok, "command", emptyArgs, false, true) },
                { Commands.Takeoff, new CommandRule(Commands.Takeoff, Responses.Ok,"takeoff", emptyArgs, false) },
                { Commands.Land, new CommandRule(Commands.Land, Responses.Ok, "land", emptyArgs, true) },
                { Commands.Stop, new CommandRule(Commands.Stop, Responses.Ok, "stop", emptyArgs, true, true) },
                { Commands.StartVideo, new CommandRule(Commands.StartVideo, Responses.Ok, "streamon", emptyArgs, false) },
                { Commands.StopVideo, new CommandRule(Commands.StopVideo, Responses.Ok, "streamoff", emptyArgs, false) },
                { Commands.EmergencyStop, new CommandRule(Commands.EmergencyStop, Responses.Ok, "emergency", emptyArgs, false, true) },

                { Commands.GetSpeed, new CommandRule(Commands.GetSpeed, Responses.Speed,"speed?", emptyArgs, false) },
                { Commands.GetBattery, new CommandRule(Commands.GetBattery, Responses.Battery, "battery?", emptyArgs, false) },
                { Commands.GetTime, new CommandRule(Commands.GetTime, Responses.Time, "time?", emptyArgs, false) },
                { Commands.GetWIFISnr, new CommandRule(Commands.GetWIFISnr, Responses.WIFISnr, "wifi?", emptyArgs, false) },
                { Commands.GetSdkVersion, new CommandRule(Commands.GetSdkVersion, Responses.SdkVersion, "sdk?", emptyArgs, false) },
                { Commands.GetSerialNumber, new CommandRule(Commands.GetSerialNumber, Responses.SerialNumber, "sn?", emptyArgs, false) },

                { Commands.Up, new CommandRule(Commands.Up, Responses.Ok,"up", movementArgs, true) },
                { Commands.Down, new CommandRule(Commands.Down, Responses.Ok, "down", movementArgs, true) },
                { Commands.Left, new CommandRule(Commands.Left, Responses.Ok, "left", movementArgs, true) },
                { Commands.Right, new CommandRule(Commands.Right, Responses.Ok, "right", movementArgs, true) },
                { Commands.Forward, new CommandRule(Commands.Forward, Responses.Ok, "forward", movementArgs, true) },
                { Commands.Back, new CommandRule(Commands.Back ,Responses.Ok, "back", movementArgs, true) },

                { Commands.ClockwiseTurn, new CommandRule(Commands.ClockwiseTurn, Responses.Ok, "cw", turnArgs, true) },
                { Commands.CounterClockwiseTurn, new CommandRule(Commands.CounterClockwiseTurn, Responses.Ok, "ccw", turnArgs, true) },

                {
                    Commands.SetSpeed, new CommandRule(Commands.SetSpeed, Responses.Ok, "speed",
                        new ArgumentRule[]
                        {
                            new IntegerRule(new IntegerRule.Range<int>(10, 100))
                        },
                        false)
                },

                {
                    Commands.Flip, new CommandRule(Commands.Flip,Responses.Ok, "flip",
                        new ArgumentRule[]
                        {
                            new CharacterRule("lrfb")
                        },
                        true)
                },

                {
                    Commands.Go, new CommandRule(Commands.Go, Responses.Ok, "go",
                        new ArgumentRule[]
                        {
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(10, 100))
                        },
                        true)
                },
                {
                    Commands.Curve, new CommandRule(Commands.Curve, Responses.Ok, "curve",
                        new ArgumentRule[]
                        {
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(10, 60))
                        },
                        true)
                },
                {
                    Commands.SetRemoteControl, new CommandRule(Commands.SetRemoteControl, Responses.None, "rc",
                        new ArgumentRule[]
                        {
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100)),
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100)),
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100)),
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100))
                        },
                        true,
                        true)
                },
                {
                    Commands.SetWiFiPassword, new CommandRule(Commands.SetWiFiPassword, Responses.Ok, "wifi",
                        new ArgumentRule[]
                        {
                            new StringRule(),
                            new StringRule()
                        },
                        false)
                },
                {
                    Commands.SetStationMode, new CommandRule(Commands.SetStationMode, Responses.Ok, "ap",
                        new ArgumentRule[]
                        {
                            new StringRule(),
                            new StringRule()
                        },
                        false)
                },

                //{Commands.SetMissionPadOn, new CommandRule(Commands.SetMissionPadOn, Responses.Ok,"", null) },
                //{Commands.SetMissionPadOff, new CommandRule(Commands.SetMissionPadOff,Responses.Ok, "", null) },
                //{Commands.SetMissionPadDirection, new CommandRule(Commands.SetMissionPadDirection, Responses.Ok,"", null) },
            };

            _rulesByString = _rulesByCommand
                .Values
                .ToDictionary((rule) => rule.Token);
        }

        private static readonly Dictionary<Commands, CommandRule> _rulesByCommand;
        private static readonly Dictionary<string, CommandRule> _rulesByString;

        public static CommandRule Rules(Commands command)
        {
            return _rulesByCommand[command];
        }

        public static CommandRule Rules(string command)
        {
            return _rulesByString[command];
        }
    }
}