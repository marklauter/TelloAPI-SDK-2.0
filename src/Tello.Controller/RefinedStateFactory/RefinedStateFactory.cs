using System;
using Tello.Messaging;

namespace Tello.Controller
{
    internal static class RefinedStateFactory
    {
        private static IPosition _position = new Position();
        private static IAirSpeed _airSpeed = new AirSpeed();
        private static IAttitude _attitude = new Attitude();
        private static IBattery _battery = new Battery();
        private static IHobbsMeter _hobbsMeter = new HobbsMeter();

        public static IRefinedDroneState GetRefinedDroneState()
        {
            return new RefinedDroneState
            {
                Position = new Position(_position),
                AirSpeed = new AirSpeed(_airSpeed),
                Attitude = new Attitude(_attitude),
                Battery = new Battery(_battery),
                HobbsMeter = new HobbsMeter(_hobbsMeter),
            };
        }

        public static void Update(IRawDroneState rawDroneState, bool useMissionPad = false)
        {
            if (rawDroneState == null)
            {
                throw new ArgumentNullException(nameof(rawDroneState));
            }

            if (useMissionPad)
            {
                if (!rawDroneState.MissionPadDetected)
                {
                    throw new ArgumentException($"{nameof(rawDroneState)}.{nameof(IRawDroneState.MissionPadDetected)} == false");
                }

                _positionGate.WithReadLock(() =>
                {
                    _position = Position.FromRawDroneState(rawDroneState, _heading);
                });
            }
            else
            {
                _positionGate.WithReadLock(() =>
                {
                    _position = Position.FromRawDroneState(rawDroneState, _x, _y, _heading);
                });
            }

            _airSpeed = AirSpeed.FromRawDroneState(rawDroneState);
            _attitude = Attitude.FromRawDroneState(rawDroneState, useMissionPad);
            _battery = Battery.FromRawDroneState(rawDroneState);
            _hobbsMeter = HobbsMeter.FromRawDroneState(rawDroneState);
        }

        #region Position State
        private static Gate _positionGate = new Gate();
        private static int _heading;
        private static double _x;
        private static double _y;

        internal static void Go(int xDistanceInCm, int yDistanceInCm)
        {
            _positionGate.WithWriteLock(() =>
            {
                _x += xDistanceInCm;
                _y += yDistanceInCm;
            });
        }

        internal static void Move(Commands direction, int distanceInCm)
        {
            _positionGate.WithWriteLock(() =>
            {
                var heading = _heading;
                switch (direction)
                {
                    case Commands.Right:
                        heading += 90;
                        break;
                    case Commands.Back:
                        heading += 180;
                        break;
                    case Commands.Left:
                        heading += 270;
                        break;
                }
                var radians = _heading * Math.PI / 180;

                var yComponent = Math.Cos(radians) * distanceInCm;
                var xComponent = Math.Sin(radians) * distanceInCm;

                _y += yComponent;
                _x += xComponent;
            });
        }

        //todo: need to experiment with yaw vs expected heading - is yaw a dynamic value?
        internal static void Turn(Commands direction, int degrees)
        {
            if (degrees < 1 || degrees > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            _positionGate.WithWriteLock(() =>
            {
                switch (direction)
                {
                    case Commands.ClockwiseTurn:
                        _heading += degrees;
                        if (_heading >= 360)
                        {
                            _heading -= 360;
                        }
                        break;
                    case Commands.CounterClockwiseTurn:
                        _heading -= degrees;
                        if (_heading < 0)
                        {
                            _heading += 360;
                        }
                        break;
                }
            });
        }
        #endregion

    }
}
