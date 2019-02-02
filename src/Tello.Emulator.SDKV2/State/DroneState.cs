using System;

namespace Tello.Emulator.SDKV2
{
    //https://dl-cdn.ryzerobotics.com/downloads/Tello/Tello%20SDK%202.0%20User%20Guide.pdf

    internal sealed class DroneState
    {
        /// <summary>
        /// speed in cm/s
        /// </summary>
        public int Speed { get; set; } = 10;

        public void TakeOff()
        {
            if (!IsFlying)
            {
                IsFlying = true;
                TakeoffTime = DateTime.Now;
                Height = 20;
            }
        }

        public void Land()
        {
            if (IsFlying)
            {
                IsFlying = false;
                Height = 0;
            }
        }

        public void StartVideo()
        {
            IsVideoOn = true;
        }

        public void StopVideo()
        {
            IsVideoOn = false;
        }

        public void ActivateSdkMode()
        {
            IsSdkModeActivated = true;
        }

        public bool IsSdkModeActivated { get; private set; } = false;
        public bool IsVideoOn { get; private set; } = false;
        public bool IsFlying { get; private set; } = false;
        public DateTime TakeoffTime { get; private set; }

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

        #region Reportable State
        public int Pitch { get; set; } = 0;

        public int Roll { get; set; } = 0;

        public int Yaw { get; set; } = 0;

        public int XSpeed { get; set; } = 0;

        public int YSpeed { get; set; } = 0;

        public int ZSpeed { get; set; } = 0;

        /// <summary>
        /// lowest temp in celsius
        /// </summary>
        public int TemperatureLow { get; set; } = 0;

        /// <summary>
        /// highest temp in celsius
        /// </summary>
        public int TemperatureHigh { get; set; } = 0;

        /// <summary>
        /// documentation says "time of flight distance in cm" - what does that mean?
        /// </summary>
        public int TimeOfFlight { get; set; } = 0;

        /// <summary>
        /// height in cm
        /// </summary>
        public int Height { get; set; } = 0;

        public int BatteryPercentage { get; set; } = 100;

        /// <summary>
        /// barometer measurement in cm
        /// </summary>
        public float BarometricPressure { get; set; } = 0;

        /// <summary>
        /// amount of time the motor has been used
        /// </summary>
        public int MotorTime { get; set; } = 0;

        /// <summary>
        /// acceleration along x axis
        /// </summary>
        public float AccelerationX { get; set; } = 0;

        /// <summary>
        /// acceleration along y axis
        /// </summary>
        public float AccelerationY { get; set; } = 0;

        /// <summary>
        /// acceleration along z axis
        /// </summary>
        public float AccelerationZ { get; set; } = 0;

        public override string ToString()
        {
            return $"pitch:{Pitch};roll:{Roll};yaw:{Yaw};vgx:{XSpeed};vgy:{YSpeed};vgz:{ZSpeed};templ:{TemperatureLow};temph:{TemperatureHigh};tof:{TimeOfFlight};h:{Height};bat:{BatteryPercentage};baro:{BarometricPressure.ToString("F2")};time:{MotorTime};agx:{AccelerationX.ToString("F2")};agy:{AccelerationY.ToString("F2")};agz:{AccelerationZ.ToString("F2")};\r\n";
        }
        #endregion
    }
}
