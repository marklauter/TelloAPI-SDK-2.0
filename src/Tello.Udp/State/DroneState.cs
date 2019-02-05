using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Tello.Messaging;

namespace Tello.Udp
{
    internal class DroneStateNameAttribute : Attribute
    {
        public DroneStateNameAttribute(string name)
        {
            Name = name;
        }

        public string Name { get; private set; }
    }

    public sealed class DroneState : IDroneState
    {
        static DroneState()
        {
            var type = typeof(DroneState);
            var properties = type
                .GetProperties()
                .Where((pi) => pi.GetCustomAttribute<DroneStateNameAttribute>() != null)
                .ToArray();

            _properties = new Dictionary<string, PropertyInfo>();
            for(var i = 0; i < properties.Length; ++i)
            {
                var property = properties[i];
                var stateName = property.GetCustomAttribute<DroneStateNameAttribute>().Name;
                _properties.Add(stateName, property);
            }
        }

        #region private static parsing helpers
        private static readonly Dictionary<string, PropertyInfo> _properties;
        private static readonly char[] _delimiters = { ':', ';' };
        private static readonly HashSet<string> _doubles = new HashSet<string>(new string[] { "baro", "agx", "agy", "agz" });
        #endregion

        public static DroneState FromDatagram(byte[] datagram)
        {
            // sample from Tello
            // mid:64;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:-7;templ:60;temph:63;tof:20;h:10;bat:89;baro:-67.44;time:0;agx:14.00;agy:-12.00;agz:-1094.00;
            // mid: 0;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:0; templ:55;temph:57;tof:10;h: 0;bat:74;baro:-42.72;time:0;agx:-8.00;agy: -1.00;agz:-1002.00;
            if (datagram == null)
            {
                throw new ArgumentNullException(nameof(datagram));
            }

            var message = Encoding.UTF8.GetString(datagram);
            var keyValues = message
                .Split(_delimiters, StringSplitOptions.RemoveEmptyEntries)
                .Where((s) => !s.Contains("\r"))
                .ToArray();
            var values = new object[_properties.Count];

            var result = new DroneState();
            for (var i = 0; i < keyValues.Length; i += 2)
            {
                var key = keyValues[i];
                var value = keyValues[i + 1];
                if (key != "mpry")
                {
                    var property = _properties[key];
                    if (_doubles.Contains(key))
                    {
                        if (Double.TryParse(keyValues[i + 1], out var parsedValue))
                        {
                            property.SetValue(result, parsedValue);
                        }
                    }
                    else
                    {
                        if (Int32.TryParse(keyValues[i + 1], out var parsedValue))
                        {
                            property.SetValue(result, parsedValue);
                        }
                    }
                }
                else
                { 
                    var mpry = keyValues[i + 1].Split(',');
                    for (var j = 0; j < mpry.Length; ++j)
                    {
                        var property = _properties[$"{key}.{j}"];
                        if (Int32.TryParse(mpry[j], out var parsedValue))
                        {
                            property.SetValue(result, parsedValue);
                        }
                    }
                } 
            }

            return result;
        }

        #region ctor
        private DroneState() { }

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
        #region mission pad
        public bool MissionPadDetected => MissionPadId != -1;
        [DroneStateName("mid")]
        public int MissionPadId { get; private set; }
        [DroneStateName("x")]
        public int MissionPadX { get; private set; }
        [DroneStateName("y")]
        public int MissionPadY { get; private set; }
        [DroneStateName("z")]
        public int MissionPadZ { get; private set; }
        [DroneStateName("mpry.0")]
        public int MissionPadPitch { get; private set; }
        [DroneStateName("mpry.1")]
        public int MissionPadRoll { get; private set; }
        [DroneStateName("mpry.2")]
        public int MissionPadYaw { get; private set; }
        #endregion
        [DroneStateName("pitch")]
        public int Pitch { get; private set; }
        [DroneStateName("roll")]
        public int Roll { get; private set; }
        [DroneStateName("yaw")]
        public int Yaw { get; private set; }
        [DroneStateName("vgx")]
        public int SpeedX { get; private set; }
        [DroneStateName("vgy")]
        public int SpeedY { get; private set; }
        [DroneStateName("vgz")]
        public int SpeedZ { get; private set; }
        [DroneStateName("templ")]
        public int TemperatureLowC { get; private set; }
        [DroneStateName("temph")]
        public int TemperatureHighC { get; private set; }
        [DroneStateName("tof")]
        public int DistanceTraversedInCm { get; private set; }
        [DroneStateName("h")]
        public int HeightInCm { get; private set; }
        [DroneStateName("bat")]
        public int BatteryPercent { get; private set; }
        [DroneStateName("time")]
        public int MotorTimeInSeconds { get; private set; }
        [DroneStateName("baro")]
        public double BarometerInCm { get; private set; }
        [DroneStateName("agx")]
        public double AccelerationX { get; private set; }
        [DroneStateName("agy")]
        public double AccelerationY { get; private set; }
        [DroneStateName("agz")]
        public double AccelerationZ { get; private set; }
        #endregion

        public override string ToString()
        {
            //mid:64;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:-7;templ:60;temph:63;tof:20;h:10;bat:89;baro:-67.44;time:0;agx:14.00;agy:-12.00;agz:-1094.00;
            var builder = new StringBuilder();

            builder.Append($"mid:{MissionPadId};");
            builder.Append($"x:{MissionPadX};");
            builder.Append($"y:{MissionPadY};");
            builder.Append($"z:{MissionPadZ};");
            builder.Append($"mpry:{MissionPadPitch},{MissionPadRoll},{MissionPadYaw};");
            builder.Append($"pitch:{Pitch};");
            builder.Append($"roll:{Roll};");
            builder.Append($"yaw:{Yaw};");
            builder.Append($"vgx:{SpeedX};");
            builder.Append($"vgy:{SpeedY};");
            builder.Append($"vgz:{SpeedZ};");
            builder.Append($"templ:{TemperatureLowC};");
            builder.Append($"temph:{TemperatureHighC};");
            builder.Append($"tof:{DistanceTraversedInCm};");
            builder.Append($"h:{HeightInCm};");
            builder.Append($"bat:{BatteryPercent};");
            builder.Append($"baro:{BarometerInCm.ToString("F2")};");
            builder.Append($"time:{MotorTimeInSeconds};");
            builder.Append($"agx:{AccelerationX.ToString("F2")};");
            builder.Append($"agy:{AccelerationY.ToString("F2")};");
            builder.Append($"agz:{AccelerationZ.ToString("F2")};");

            return builder.ToString();
        }

    }
}
