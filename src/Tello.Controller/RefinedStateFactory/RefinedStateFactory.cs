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

        public static IPosition GetPosition()
        {
            return new Position(_position);
        }

        public static IAirSpeed GetAirSpeed()
        {
            return new AirSpeed(_airSpeed);
        }

        public static IAttitude GetAttitude()
        {
            return new Attitude(_attitude);
        }

        public static IBattery GetBattery()
        {
            return new Battery(_battery);
        }

        public static IHobbsMeter GetHobbsMeter()
        {
            return new HobbsMeter(_hobbsMeter);
        }

        public static IRefinedDroneState GetRefinedDroneState()
        {
            return new RefinedDroneState(
                new Position(_position),
                new Attitude(_attitude),
                new AirSpeed(_airSpeed),
                new Battery(_battery),
                new HobbsMeter(_hobbsMeter));
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
                    _position = new Position(rawDroneState, _heading);
                });
            }
            else
            {
                _positionGate.WithReadLock(() =>
                {
                    _position = new Position(rawDroneState, _x, _y, _heading);
                });
            }

            _airSpeed = new AirSpeed(rawDroneState);
            _attitude = new Attitude(rawDroneState, useMissionPad);
            _battery = new Battery(rawDroneState);
            _hobbsMeter = new HobbsMeter(rawDroneState);
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

        // note: X is forward
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

                var adjacent = Math.Cos(radians) * distanceInCm;
                var opposite = Math.Sin(radians) * distanceInCm;

                _x += adjacent;
                _y += opposite;
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
