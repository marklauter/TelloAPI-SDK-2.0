using System;
using System.Diagnostics;
using Tello.Messaging;

namespace Tello.Controller
{
    /// <summary>
    /// todo: make thread safe
    /// </summary>
    public class Position
    {
        private Position() { }

        public static Position GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Position();
            }
            return _instance;
        }

        private static Position _instance = null;
        private static double _barometerDelta;

        public static void ZeroAltimeter(double baromerInCm, double altMslInCm)
        {
            _barometerDelta = altMslInCm - baromerInCm;
        }

        public void Update(IRawDroneState droneState, bool useMissionPad = false)
        {
            if (useMissionPad)
            {
                X = droneState.MissionPadX;
                Y = droneState.MissionPadY;
                Yaw = droneState.MissionPadYaw;
                AltAGL = droneState.MissionPadZ;
            }
            else
            {
                Yaw = droneState.Yaw;
                AltAGL = droneState.HeightInCm;
            }

            AltMSL = droneState.BarometerInCm + _barometerDelta;
        }

        public double X { get; private set; }
        public double Y { get; private set; }

        /// <summary>
        /// uses barometer to estimate altitude above mean sea level
        /// a reading must be taken to be used as the zero reading - initialize this with the static ZeroAltimeter method
        /// </summary>
        public double AltMSL { get; private set; }

        /// <summary>
        /// alt above ground level
        /// </summary>
        public int AltAGL { get; private set; }

        public int Heading { get; private set; }
        public int Yaw { get; private set; }

        internal void Go(int x, int y)
        {
            X += x;
            Y += y;
        }

        internal void Move(Commands direction, int distanceInCm)
        {
            //Debug.WriteLine($"move {direction}, distance {distanceInCm}, heading {Heading}");

            var heading = Heading;
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
            var radians = Heading * Math.PI / 180;

            var yComponent = Math.Cos(radians) * distanceInCm;
            var xComponent = Math.Sin(radians) * distanceInCm;
            //Debug.WriteLine($"components (X,Y): ({xComponent.ToString("F2")},{yComponent.ToString("F2")})");
            
            Y += yComponent;
            X += xComponent;
            //Debug.WriteLine($"New (X,Y): ({X.ToString("F2")},{Y.ToString("F2")})");
        }

        internal void Turn(Commands direction, int degrees)
        {
            if (degrees < 1 || degrees > 360)
            {
                throw new ArgumentOutOfRangeException(nameof(degrees));
            }

            //Debug.WriteLine($"Old Heading: {Heading}");
            switch (direction)
            {
                case Commands.ClockwiseTurn:
                    Heading += degrees;
                    if (Heading >= 360)
                    {
                        Heading -= 360;
                    }
                    break;
                case Commands.CounterClockwiseTurn:
                    Heading -= degrees;
                    if (Heading < 0)
                    {
                        Heading += 360;
                    }
                    break;
            }
            //Debug.WriteLine($"New Heading: {Heading}");
        }

        public override string ToString()
        {
            var mslFt = AltMSL / 30.48;
            return $"X:{X.ToString("F2")}, Y:{Y.ToString("F2")}, AMSL:{mslFt.ToString("F2")}, AGL {AltAGL}, Hd:{Heading}, Bd:{_barometerDelta.ToString("F2")}";
        }
    }
}
