using System.Diagnostics;
using System.Text;
using Tello.Messaging;

namespace Tello.Emulator.SDKV2
{
    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

    internal sealed class RawDroneStateEmulated : IRawDroneState
    {
        internal Stopwatch MotorClock { get; private set; }
        private readonly int _motorTime;

        internal RawDroneStateEmulated()
        {
            MotorClock = new Stopwatch();
            BatteryPercent = 100;
        }

        internal RawDroneStateEmulated(IRawDroneState droneState)
        {
            MotorClock = null;
            _motorTime = droneState.MotorTimeInSeconds;
            Pitch = droneState.Pitch;
            Roll = droneState.Roll;
            Yaw = droneState.Yaw;
            SpeedX = droneState.SpeedX;
            SpeedY = droneState.SpeedY;
            SpeedZ = droneState.SpeedZ;
            TemperatureLowC = droneState.TemperatureLowC;
            TemperatureHighC = droneState.TemperatureHighC;
            DistanceTraversedInCm = droneState.DistanceTraversedInCm;
            HeightInCm = droneState.HeightInCm;
            BatteryPercent = droneState.BatteryPercent;
            BarometerInCm = droneState.BarometerInCm;
            AccelerationX = droneState.AccelerationX;
            AccelerationY = droneState.AccelerationY;
            AccelerationZ = droneState.AccelerationZ;
        }

        #region Mission Pad
        public bool MissionPadDetected => MissionPadId != -1;
        public int MissionPadId => -1;
        public int MissionPadX => 0;
        public int MissionPadY => 0;
        public int MissionPadZ => 0;
        public int MissionPadPitch => 0;
        public int MissionPadRoll => 0;
        public int MissionPadYaw => 0;
        #endregion
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
        public int MotorTimeInSeconds => MotorClock != null
            ? (int)MotorClock.Elapsed.TotalSeconds
            : _motorTime;
        public double BarometerInCm { get; internal set; }
        public double AccelerationX { get; internal set; }
        public double AccelerationY { get; internal set; }
        public double AccelerationZ { get; internal set; }


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
            builder.Append($"alt:{BarometerInCm.ToString("F2")};");
            builder.Append($"time:{MotorTimeInSeconds};");
            builder.Append($"agx:{AccelerationX.ToString("F2")};");
            builder.Append($"agy:{AccelerationY.ToString("F2")};");
            builder.Append($"agz:{AccelerationZ.ToString("F2")};");

            return builder.ToString();
        }
    }
}
