// <copyright file="TelloState.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Tello.State
{
    public class TelloState : ITelloState
    {
        #region parsing support
        private class StatePropertyAttribute : Attribute
        {
            public StatePropertyAttribute(string name)
            {
                if (String.IsNullOrEmpty(name))
                {
                    throw new ArgumentNullException(nameof(name));
                }

                this.Name = name;
            }

            public string Name { get; private set; }
        }

        static TelloState()
        {
            var properties =
                typeof(TelloState)
                .GetProperties()
                .Where(pi => pi.GetCustomAttribute<StatePropertyAttribute>() != null);

            TelloState.Properties = new Dictionary<string, PropertyInfo>();
            foreach (var property in properties)
            {
                var stateName = property.GetCustomAttribute<StatePropertyAttribute>().Name;
                TelloState.Properties.Add(stateName, property);
            }
        }

        private static readonly Dictionary<string, PropertyInfo> Properties;
        private static readonly char[] Delimiters = { ':', ';' };
        private static readonly HashSet<string> Doubles = new HashSet<string>(new string[] { "baro", "agx", "agy", "agz" });
        #endregion

        private readonly Vector vector;

        public TelloState()
            : this(new Vector())
        {
            this.Timestamp = DateTime.UtcNow;
            this.Data = String.Empty;
        }

        public TelloState(Vector position)
        {
            this.Timestamp = DateTime.UtcNow;
            this.Data = String.Empty;
            this.vector = position;
        }

        public TelloState(string state)
            : this(state, DateTime.UtcNow, new Vector())
        {
        }

        public TelloState(string state, Vector position)
            : this(state, DateTime.UtcNow, position)
        {
        }

        public TelloState(string state, DateTime timestamp)
            : this(state, timestamp, new Vector())
        {
        }

        public TelloState(string state, DateTime timestamp, Vector position)
        {
            // sample from Tello
            // mid:64;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:-7;templ:60;temph:63;tof:20;h:10;bat:89;baro:-67.44;time:0;agx:14.00;agy:-12.00;agz:-1094.00;
            // mid: 0;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:0; templ:55;temph:57;tof:10;h: 0;bat:74;baro:-42.72;time:0;agx:-8.00;agy: -1.00;agz:-1002.00;
            if (String.IsNullOrEmpty(state))
            {
                throw new ArgumentNullException(nameof(state));
            }

            this.Timestamp = timestamp;
            this.Data = state;
            this.vector = position;

            var keyValues = state
                .Split(Delimiters, StringSplitOptions.RemoveEmptyEntries)
                .Where(s => !s.Contains("\r"))
                .Select(s => s.Trim())
                .ToArray();

            var values = new object[Properties.Count];

            for (var i = 0; i < keyValues.Length; i += 2)
            {
                var key = keyValues[i];
                var value = keyValues[i + 1];

                if (key != "mpry")
                {
                    var property = Properties[key];
                    if (Doubles.Contains(key))
                    {
                        if (Double.TryParse(keyValues[i + 1], out var parsedValue))
                        {
                            property.SetValue(this, parsedValue);
                        }
                    }
                    else
                    {
                        if (Int32.TryParse(keyValues[i + 1], out var parsedValue))
                        {
                            property.SetValue(this, parsedValue);
                        }
                    }
                }
                else
                {
                    var mpryValues = keyValues[i + 1].Split(',');
                    for (var j = 0; j < mpryValues.Length; ++j)
                    {
                        var property = Properties[$"{key}.{j}"];
                        if (Int32.TryParse(mpryValues[j], out var parsedValue))
                        {
                            property.SetValue(this, parsedValue);
                        }
                    }
                }
            }
        }

        public string Data { get; }

        public DateTime Timestamp { get; }

        public IAirSpeed AirSpeed => new AirSpeed(this);

        public IAttitude Attitude => new Attitude(this);

        public IBattery Battery => new Battery(this);

        public IHobbsMeter HobbsMeter => new HobbsMeter(this);

        public IPosition Position =>
            this.vector != null
            ? new Position(this, this.vector)
            : new Position(this);

        #region mission pad
        public bool MissionPadDetected => this.MissionPadId != -1;

        [StateProperty("mid")]
        public int MissionPadId { get; private set; }

        [StateProperty("x")]
        public int MissionPadX { get; private set; }

        [StateProperty("y")]
        public int MissionPadY { get; private set; }

        [StateProperty("z")]
        public int MissionPadZ { get; private set; }

        [StateProperty("mpry.0")]
        public int MissionPadPitch { get; private set; }

        [StateProperty("mpry.1")]
        public int MissionPadRoll { get; private set; }

        [StateProperty("mpry.2")]
        public int MissionPadYaw { get; private set; }

        #endregion
        [StateProperty("pitch")]
        public int Pitch { get; private set; }

        [StateProperty("roll")]
        public int Roll { get; private set; }

        [StateProperty("yaw")]
        public int Yaw { get; private set; }

        [StateProperty("vgx")]
        public int SpeedX { get; private set; }

        [StateProperty("vgy")]
        public int SpeedY { get; private set; }

        [StateProperty("vgz")]
        public int SpeedZ { get; private set; }

        [StateProperty("templ")]
        public int TemperatureLowC { get; private set; }

        [StateProperty("temph")]
        public int TemperatureHighC { get; private set; }

        [StateProperty("tof")]
        public int DistanceTraversedInCm { get; private set; }

        [StateProperty("h")]
        public int HeightInCm { get; private set; }

        [StateProperty("bat")]
        public int BatteryPercent { get; private set; }

        [StateProperty("time")]
        public int MotorTimeInSeconds { get; private set; }

        [StateProperty("baro")]
        public double BarometerInCm { get; private set; }

        [StateProperty("agx")]
        public double AccelerationX { get; private set; }

        [StateProperty("agy")]
        public double AccelerationY { get; private set; }

        [StateProperty("agz")]
        public double AccelerationZ { get; private set; }

        public override string ToString()
        {
            // mid:64;x:0;y:0;z:0;mpry:0,0,0;pitch:0;roll:0;yaw:0;vgx:0;vgy:0;vgz:-7;templ:60;temph:63;tof:20;h:10;bat:89;baro:-67.44;time:0;agx:14.00;agy:-12.00;agz:-1094.00;
            var builder = new StringBuilder();

            builder.Append($"mid:{this.MissionPadId};");
            builder.Append($"x:{this.MissionPadX};");
            builder.Append($"y:{this.MissionPadY};");
            builder.Append($"z:{this.MissionPadZ};");
            builder.Append($"mpry:{this.MissionPadPitch},{this.MissionPadRoll},{this.MissionPadYaw};");
            builder.Append($"pitch:{this.Pitch};");
            builder.Append($"roll:{this.Roll};");
            builder.Append($"yaw:{this.Yaw};");
            builder.Append($"vgx:{this.SpeedX};");
            builder.Append($"vgy:{this.SpeedY};");
            builder.Append($"vgz:{this.SpeedZ};");
            builder.Append($"templ:{this.TemperatureLowC};");
            builder.Append($"temph:{this.TemperatureHighC};");
            builder.Append($"tof:{this.DistanceTraversedInCm};");
            builder.Append($"h:{this.HeightInCm};");
            builder.Append($"bat:{this.BatteryPercent};");
            builder.Append($"baro:{this.BarometerInCm.ToString("F2")};");
            builder.Append($"time:{this.MotorTimeInSeconds};");
            builder.Append($"agx:{this.AccelerationX.ToString("F2")};");
            builder.Append($"agy:{this.AccelerationY.ToString("F2")};");
            builder.Append($"agz:{this.AccelerationZ.ToString("F2")};");

            return builder.ToString();
        }
    }
}
