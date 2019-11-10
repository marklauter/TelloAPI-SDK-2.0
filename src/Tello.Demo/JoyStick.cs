// <copyright file="JoyStick.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;

namespace Tello.Demo
{
    public class JoyStick
    {
        public enum ScanStates
        {
            Changed,
            Unchanged,
            Exit,
        }

        private readonly int maxInputTimeoutInMS = 100;
        private readonly Stopwatch xTimer = new Stopwatch();
        private readonly Stopwatch yTimer = new Stopwatch();
        private readonly Stopwatch zTimer = new Stopwatch();
        private readonly Stopwatch rTimer = new Stopwatch();
        private int xInput = 0;
        private int yInput = 0;
        private int zInput = 0;
        private int rInput = 0;

        /// <summary>
        /// Left/Right
        /// </summary>
        public int XInput => this.xInput;

        /// <summary>
        /// Up/Down
        /// </summary>
        public int YInput => this.yInput;

        /// <summary>
        /// Forward/Backward
        /// </summary>
        public int ZInput => this.zInput;

        /// <summary>
        /// Yaw
        /// </summary>
        public int RInput => this.rInput;

        public ScanStates Scan()
        {
            var timerState = this.TimerCheck();
            var keyboardState = ScanStates.Unchanged;

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.W: // forward
                        keyboardState = this.Forward();
                        break;
                    case ConsoleKey.S: // backward
                        keyboardState = this.Backward();
                        break;
                    case ConsoleKey.D: // right
                        keyboardState = this.Right();
                        break;
                    case ConsoleKey.A: // left
                        keyboardState = this.Left();
                        break;
                    case ConsoleKey.UpArrow: // up
                        keyboardState = this.Up();
                        break;
                    case ConsoleKey.DownArrow: // down
                        keyboardState = this.Down();
                        break;
                    case ConsoleKey.LeftArrow: // turn left
                        keyboardState = this.YawLeft();
                        break;
                    case ConsoleKey.RightArrow: // right
                        keyboardState = this.YawRight();
                        break;
                    case ConsoleKey.Escape: // exit
                        return ScanStates.Exit;
                    default:
                        break;
                }
            }

            return timerState == ScanStates.Changed || keyboardState == ScanStates.Changed
                ? ScanStates.Changed
                : ScanStates.Unchanged;
        }

        private ScanStates Center(ref int input, Stopwatch timer)
        {
            var ms = timer.IsRunning ? timer.ElapsedMilliseconds : 0;
            if (ms > this.maxInputTimeoutInMS)
            {
                timer.Reset();
                input = 0;
                return ScanStates.Changed;
            }

            return ScanStates.Unchanged;
        }

        private ScanStates CenterX()
        {
            return this.Center(ref this.xInput, this.xTimer);
        }

        private ScanStates CenterY()
        {
            return this.Center(ref this.yInput, this.yTimer);
        }

        private ScanStates CenterZ()
        {
            return this.Center(ref this.zInput, this.zTimer);
        }

        private ScanStates CenterR()
        {
            return this.Center(ref this.rInput, this.rTimer);
        }

        private ScanStates TimerCheck()
        {
            var xState = this.CenterX();
            var yState = this.CenterY();
            var zState = this.CenterZ();
            var rState = this.CenterR();

            return xState == ScanStates.Changed || yState == ScanStates.Changed || zState == ScanStates.Changed || rState == ScanStates.Changed
                ? ScanStates.Changed
                : ScanStates.Unchanged;
        }

        private ScanStates Move(ref int input, int direction, Stopwatch timer)
        {
            var last = input;
            if (Math.Abs(direction) == direction) // positive
            {
                input = input < 0
                    ? 0
                    : input < 100
                        ? input + 10
                        : input;
            }
            else // negative
            {
                input = input > 0
                    ? 0
                    : input > -100
                        ? input - 10
                        : input;
            }

            timer.Restart();
            return last == input ? ScanStates.Unchanged : ScanStates.Changed;
        }

        private ScanStates Right()
        {
            return this.Move(ref this.xInput, 1, this.xTimer);
        }

        private ScanStates Left()
        {
            return this.Move(ref this.xInput, -1, this.xTimer);
        }

        private ScanStates Forward()
        {
            return this.Move(ref this.zInput, 1, this.zTimer);
        }

        private ScanStates Backward()
        {
            return this.Move(ref this.zInput, -1, this.zTimer);
        }

        private ScanStates Up()
        {
            return this.Move(ref this.yInput, 1, this.yTimer);
        }

        private ScanStates Down()
        {
            return this.Move(ref this.yInput, -1, this.yTimer);
        }

        private ScanStates YawRight()
        {
            return this.Move(ref this.rInput, 1, this.rTimer);
        }

        private ScanStates YawLeft()
        {
            return this.Move(ref this.rInput, -1, this.rTimer);
        }
    }
}
