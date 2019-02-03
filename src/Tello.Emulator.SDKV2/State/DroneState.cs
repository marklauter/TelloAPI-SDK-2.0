using System.Diagnostics;
using System.Text;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

    internal sealed class DroneState : IDroneState
    {
        internal Stopwatch MotorClock { get; }

        internal DroneState()
        {
            MotorClock = new Stopwatch();
        }

        public int MissionPadId => -1;
        public int MissionPadX => 0;
        public int MissionPadY => 0;
        public int MissionPadZ => 0;
        public int MissionPadPitch => 0;
        public int MissionPadRoll => 0;
        public int MissionPadYaw => 0;
        public int Pitch { get; internal set; }
        public int Roll { get; internal set; }
        public int Yaw { get; internal set; }
        public int SpeedX { get; internal set; }
        public int SpeedY { get; internal set; }
        public int SpeedZ { get; internal set; }
        public int TemperatureLowC { get; internal set; }
        public int TemperatureHighC { get; internal set; }
        public int DistanceTraversedInCm { get; internal set; }
        public int HeightInCm { get; internal set; }
        public int BatteryPercent { get; internal set; }
        public double BarometerInCm { get; internal set; }
        public int MotorTimeInSeconds => (int)MotorClock.Elapsed.TotalSeconds;
        public double AccelerationX { get; internal set; }
        public double AccelerationY { get; internal set; }
        public double AccelerationZ { get; internal set; }
        public bool MissionPadDetected => MissionPadId != -1;

        #region Mission Pad
        //[JsonProperty("mid")]
        //public int MissionPadDected { get; set; } = -1;

        //[JsonProperty("x")]
        //public double MissionPadX { get; set; } = 0;

        //[JsonProperty("y")]
        //public double MissionPadY { get; set; } = 0;

        //[JsonProperty("z")]
        //public double MissionPadZ { get; set; } = 0;
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
