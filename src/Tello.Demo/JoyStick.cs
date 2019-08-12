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

        private readonly int _maxMS = 250;
        private readonly Stopwatch _xTimer = new Stopwatch();
        private readonly Stopwatch _yTimer = new Stopwatch();
        private readonly Stopwatch _zTimer = new Stopwatch();
        private readonly Stopwatch _rTimer = new Stopwatch();
        private int _xInput = 0;
        private int _yInput = 0;
        private int _zInput = 0;
        private int _rInput = 0;

        /// <summary>
        /// Left/Right
        /// </summary>
        public int XInput => _xInput;

        /// <summary>
        /// Up/Down
        /// </summary>
        public int YInput => _yInput;

        /// <summary>
        /// Forward/Backward
        /// </summary>
        public int ZInput => _zInput;

        /// <summary>
        /// Yaw
        /// </summary>
        public int RInput => _rInput;

        public ScanStates Scan()
        {
            var timerState = TimerCheck();
            var keyboardState = ScanStates.Unchanged;

            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true).Key;
                switch (key)
                {
                    case ConsoleKey.W: // forward
                        keyboardState = Forward();
                        break;
                    case ConsoleKey.S: // backward
                        keyboardState = Backward();
                        break;
                    case ConsoleKey.D: // right
                        keyboardState = Right();
                        break;
                    case ConsoleKey.A: // left
                        keyboardState = Left();
                        break;
                    case ConsoleKey.UpArrow: // up
                        keyboardState = Up();
                        break;
                    case ConsoleKey.DownArrow: // down
                        keyboardState = Down();
                        break;
                    case ConsoleKey.LeftArrow: // turn left
                        keyboardState = YawLeft();
                        break;
                    case ConsoleKey.RightArrow: // right
                        keyboardState = YawRight();
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
            if (ms > _maxMS)
            {
                timer.Reset();
                input = 0;
                return ScanStates.Changed;
            }
            return ScanStates.Unchanged;
        }

        private ScanStates CenterX()
        {
            return Center(ref _xInput, _xTimer);
        }

        private ScanStates CenterY()
        {
            return Center(ref _yInput, _yTimer);
        }

        private ScanStates CenterZ()
        {
            return Center(ref _zInput, _zTimer);
        }

        private ScanStates CenterR()
        {
            return Center(ref _rInput, _rTimer);
        }

        private ScanStates TimerCheck()
        {
            var xState = CenterX();
            var yState = CenterY();
            var zState = CenterZ();
            var rState = CenterR();

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
            return Move(ref _xInput, 1, _xTimer);
        }

        private ScanStates Left()
        {
            return Move(ref _xInput, -1, _xTimer);
        }

        private ScanStates Forward()
        {
            return Move(ref _zInput, 1, _zTimer);
        }

        private ScanStates Backward()
        {
            return Move(ref _zInput, -1, _zTimer);
        }

        private ScanStates Up()
        {
            return Move(ref _yInput, 1, _yTimer);
        }

        private ScanStates Down()
        {
            return Move(ref _yInput, -1, _yTimer);
        }

        private ScanStates YawRight()
        {
            return Move(ref _rInput, 1, _rTimer);
        }

        private ScanStates YawLeft()
        {
            return Move(ref _rInput, -1, _rTimer);
        }
    }
}
