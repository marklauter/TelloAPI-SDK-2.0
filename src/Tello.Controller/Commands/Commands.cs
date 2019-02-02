﻿namespace Tello.Controller
{
    public enum ValueResponseTypes
    {
        Ok,
        Speed,
        Battery,
        Time,
        WiFiSnr,
        SdkVersion,
        SerialNumber
    }

    internal enum Commands
    {
        // ------------------------
        // control commands
        // ------------------------
        // no args
        EstablishLink,
        Takeoff,
        Land,
        Stop,
        StartVideo,
        StopVideo,
        EmergencyStop,

        // single args
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back,
        ClockwiseTurn,
        CounterClockwiseTurn,
        Flip,

        // multi args
        Go,
        Curve,

        // ------------------------
        // set commands
        // ------------------------
        SetSpeed,
        SetRemoteControl,
        SetWiFiPassword,
        //SetMissionPadOn,
        //SetMissionPadOff,
        //SetMissionPadDirection,
        SetStationMode,

        // ------------------------
        // read commands
        // ------------------------
        GetSpeed,
        GetBattery,
        GetTime,
        GetWiFiSnr,
        GetSdkVersion,
        GetSerialNumber,
    }
}
