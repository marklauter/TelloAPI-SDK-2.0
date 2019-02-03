using System;
using System.Collections.Generic;
using System.Text;
using Tello.Messaging;

namespace Tello.Controller.State
{
    //mid:64;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:-7;templ:60;temph:63;tof:20;h:10;bat:89;baro:-67.44;time:0;agx:14.00;agy:-12.00;agz:-1094.00;
    //todo: the current implentation works from the order of the parameters and ignores the names - better to use the names to set the values - maybe a hashtable that points to the property or something
    internal sealed class DroneState : IDroneState
    {
        #region private static parsing helpers
        private static readonly char[] _delimiters = { ':', ';' };
        private static readonly HashSet<string> _doubles = new HashSet<string>(new string[] { "baro", "agx", "agy", "agz" });
        #endregion

        public static DroneState FromDatagram(byte[] datagram)
        {
            if (datagram == null)
            {
                throw new ArgumentNullException(nameof(datagram));
            }

            var message = Encoding.UTF8.GetString(datagram);
            var keyValuePairs = message.Split(_delimiters);
            var values = new object[23];

            var valueIndex = 0;
            for (var i = 0; i < keyValuePairs.Length; i += 2)
            {
                if (keyValuePairs[i] == "mpry")
                {
                    var mpry = keyValuePairs[i + 1].Split(',');
                    for (var j = 0; j < mpry.Length; ++j)
                    {
                        if (Int32.TryParse(mpry[j], out var value))
                        {
                            values[valueIndex++] = value;
                        }
                    }
                }
                else if (_doubles.Contains( keyValuePairs[i]))
                {
                    if (Double.TryParse(keyValuePairs[i + 1], out var value))
                    {
                        values[valueIndex++] = value;
                    }
                }
                else
                {
                    if (Int32.TryParse(keyValuePairs[i + 1], out var value))
                    {
                        values[valueIndex++] = value;
                    }
                }
            }

            return new DroneState(
                (int)values[0],
                (int)values[1],
                (int)values[2],
                (int)values[3],
                (int)values[4],
                (int)values[5],
                (int)values[6],
                (int)values[7],
                (int)values[8],
                (int)values[9],
                (int)values[10],
                (int)values[11],
                (int)values[12],
                (int)values[13],
                (int)values[14],
                (int)values[15],
                (int)values[16],
                (int)values[17],
                (double)values[18],
                (int)values[19],
                (double)values[20],
                (double)values[21],
                (double)values[22]);
        }

        #region ctor
        private DroneState(
            int missionPadId,
            int missionPadX,
            int missionPadY,
            int missionPadZ,
            int missionPadPitch,
            int missionPadRoll,
            int missionPadYaw,
            int pitch,
            int roll,
            int yaw,
            int speedX,
            int speedY,
            int speedZ,
            int temperatureLowC,
            int temperatureHighC,
            int distanceTraversedInCm,
            int heightInCm,
            int batteryPercent,
            double barometerInCm,
            int motorTimeInSeconds,
            double accelerationX,
            double accelerationY,
            double accelerationZ)
        {
            MissionPadId = missionPadId;
            MissionPadX = missionPadX;
            MissionPadY = missionPadY;
            MissionPadZ = missionPadZ;
            MissionPadPitch = missionPadPitch;
            MissionPadRoll = missionPadRoll;
            MissionPadYaw = missionPadYaw;
            Pitch = pitch;
            Roll = roll;
            Yaw = yaw;
            SpeedX = speedX;
            SpeedY = speedY;
            SpeedZ = speedZ;
            TemperatureLowC = temperatureLowC;
            TemperatureHighC = temperatureHighC;
            DistanceTraversedInCm = distanceTraversedInCm;
            HeightInCm = heightInCm;
            BatteryPercent = batteryPercent;
            BarometerInCm = Math.Abs(barometerInCm);
            MotorTimeInSeconds = motorTimeInSeconds;
            AccelerationX = accelerationX;
            AccelerationY = accelerationY;
            AccelerationZ = accelerationZ;
        }
        #endregion

        #region public properties
        public int MissionPadId { get; }
        public int MissionPadX { get; }
        public int MissionPadY { get; }
        public int MissionPadZ { get; }
        public int MissionPadPitch { get; }
        public int MissionPadRoll { get; }
        public int MissionPadYaw { get; }

        public int Pitch { get; }
        public int Roll { get; }
        public int Yaw { get; }
        public int SpeedX { get; }
        public int SpeedY { get; }
        public int SpeedZ { get; }
        public int TemperatureLowC { get; }
        public int TemperatureHighC { get; }
        public int DistanceTraversedInCm { get; }
        public int HeightInCm { get; }
        public int BatteryPercent { get; }
        public double BarometerInCm { get; }
        public int MotorTimeInSeconds { get; }
        public double AccelerationX { get; }
        public double AccelerationY { get; }
        public double AccelerationZ { get; }

        public bool MissionPadDetected => MissionPadId != -1;
        #endregion
    }
}
