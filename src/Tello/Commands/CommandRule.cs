// <copyright file="CommandRule.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

// ------------ error messages I've seen -------------
// error
// error Motor Stop
// error Not Joystick
// error Auto land
namespace Tello
{
    /// <summary>
    /// CommandRule governs the exection of a command.
    /// </summary>
    public sealed class CommandRule
    {
        internal CommandRule(
            Commands command,
            Responses expectedResponse,
            string token,
            ArgumentRule[] arguments,
            bool isInFlightRequired,
            bool isImmediate = false)
        {
            this.Command = command;
            this.ExpectedResponse = expectedResponse;
            this.Token = token;
            this.Arguments = arguments;
            this.IsInFlightRequired = isInFlightRequired;
            this.IsImmediate = isImmediate;
        }

        public Commands Command { get; }

        public Responses ExpectedResponse { get; }

        public string Token { get; }

        public ArgumentRule[] Arguments { get; }

        public bool IsInFlightRequired { get; }

        public bool IsImmediate { get; }

        public bool IsBatch => !IsImmediate;

        public string ToString(params object[] args)
        {
            var result = this.Token;

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
}