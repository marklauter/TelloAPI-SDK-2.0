﻿using System;
using System.Collections.Generic;
using Tello.Types;

namespace Tello
{
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
            bool immediate = false)
        {
            Command = command;
            Response = response;
            Token = token;
            Arguments = arguments;
            Immediate = immediate;
        }

        public readonly Commands Command;
        public readonly string Token;
        public readonly ArgumentRule[] Arguments;
        public readonly Responses Response;
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
                { Commands.EnterSdkMode, new CommandRule(Commands.EnterSdkMode, Responses.Ok, "command", emptyArgs, true) },
                { Commands.Takeoff, new CommandRule(Commands.Takeoff, Responses.Ok,"takeoff", emptyArgs) },
                { Commands.Land, new CommandRule(Commands.Land, Responses.Ok, "land", emptyArgs) },
                { Commands.Stop, new CommandRule(Commands.Stop, Responses.Ok, "stop", emptyArgs, true) },
                { Commands.StartVideo, new CommandRule(Commands.StartVideo, Responses.Ok, "streamon", emptyArgs) },
                { Commands.StopVideo, new CommandRule(Commands.StopVideo, Responses.Ok, "streamoff", emptyArgs) },
                { Commands.EmergencyStop, new CommandRule(Commands.EmergencyStop, Responses.Ok, "emergency", emptyArgs, true) },

                { Commands.GetSpeed, new CommandRule(Commands.GetSpeed, Responses.Speed,"speed?", emptyArgs) },
                { Commands.GetBattery, new CommandRule(Commands.GetBattery, Responses.Battery, "battery?", emptyArgs) },
                { Commands.GetTime, new CommandRule(Commands.GetTime, Responses.Time, "time?", emptyArgs) },
                { Commands.GetWIFISnr, new CommandRule(Commands.GetWIFISnr, Responses.WIFISnr, "wifi?", emptyArgs) },
                { Commands.GetSdkVersion, new CommandRule(Commands.GetSdkVersion, Responses.SdkVersion, "sdk?", emptyArgs) },
                { Commands.GetSerialNumber, new CommandRule(Commands.GetSerialNumber, Responses.SerialNumber, "sn?", emptyArgs) },

                { Commands.Up, new CommandRule(Commands.Up, Responses.Ok,"up", movementArgs) },
                { Commands.Down, new CommandRule(Commands.Down, Responses.Ok, "down", movementArgs) },
                { Commands.Left, new CommandRule(Commands.Left, Responses.Ok, "left", movementArgs) },
                { Commands.Right, new CommandRule(Commands.Right, Responses.Ok, "right", movementArgs) },
                { Commands.Forward, new CommandRule(Commands.Forward, Responses.Ok, "forward", movementArgs) },
                { Commands.Back, new CommandRule(Commands.Back ,Responses.Ok, "back", movementArgs) },

                { Commands.ClockwiseTurn, new CommandRule(Commands.ClockwiseTurn, Responses.Ok, "cw", turnArgs) },
                { Commands.CounterClockwiseTurn, new CommandRule(Commands.CounterClockwiseTurn, Responses.Ok, "ccw", turnArgs) },

                {
                    Commands.SetSpeed, new CommandRule(Commands.SetSpeed, Responses.Ok, "speed",
                        new ArgumentRule[]
                        {
                            new IntegerRule(new IntegerRule.Range<int>(10, 100))
                        })
                },

                {
                    Commands.Flip, new CommandRule(Commands.Flip,Responses.Ok, "flip",
                        new ArgumentRule[]
                        {
                            new CharacterRule("lrfb")
                        })
                },

                {
                    Commands.Go, new CommandRule(Commands.Go, Responses.Ok, "go",
                        new ArgumentRule[]
                        {
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(-500, 500)),
                            new IntegerRule(new IntegerRule.Range<int>(10, 100))
                        })
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
                        })
                },
                {
                    Commands.SetRemoteControl, new CommandRule(Commands.SetRemoteControl, Responses.Ok, "rc",
                        new ArgumentRule[]
                        {
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100)),
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100)),
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100)),
                            new IntegerRule(new IntegerRule.Range<int>(-100, 100))
                        })
                },
                {
                    Commands.SetWiFiPassword, new CommandRule(Commands.SetWiFiPassword, Responses.Ok, "wifi",
                        new ArgumentRule[]
                        {
                            new StringRule(),
                            new StringRule()
                        })
                },
                {
                    Commands.SetStationMode, new CommandRule(Commands.SetStationMode, Responses.Ok, "ap",
                        new ArgumentRule[]
                        {
                            new StringRule(),
                            new StringRule()
                        })
                },

                //{Commands.SetMissionPadOn, new CommandRule(Commands.SetMissionPadOn, Responses.Ok,"", null) },
                //{Commands.SetMissionPadOff, new CommandRule(Commands.SetMissionPadOff,Responses.Ok, "", null) },
                //{Commands.SetMissionPadDirection, new CommandRule(Commands.SetMissionPadDirection, Responses.Ok,"", null) },
            };

            _rulesByString = new Dictionary<string, CommandRule>(_rulesByCommand.Count);
            foreach (var rule in _rulesByCommand.Values)
            {
                _rulesByString.Add(rule.Token, rule);
            }
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