using System;
using System.Collections.Generic;
using System.Text;

namespace Tello.Emulator.SDKV2.State
{
    public sealed class FlightController
    {
        public DroneState DroneState { get; }



        //public void TakeOff()
        //{
        //    if (!IsFlying)
        //    {
        //        IsFlying = true;
        //        TakeoffTime = DateTime.Now;
        //        Height = 20;
        //    }
        //}

        //public void Land()
        //{
        //    if (IsFlying)
        //    {
        //        IsFlying = false;
        //        Height = 0;
        //    }
        //}

        //public void StartVideo()
        //{
        //    IsVideoOn = true;
        //}

        //public void StopVideo()
        //{
        //    IsVideoOn = false;
        //}

        //public void ActivateSdkMode()
        //{
        //    IsSdkModeActivated = true;
        //}

        //public bool IsSdkModeActivated { get; private set; } = false;
        //public bool IsVideoOn { get; private set; } = false;
        //public bool IsFlying { get; private set; } = false;
        //public DateTime TakeoffTime { get; private set; }

    }
}
