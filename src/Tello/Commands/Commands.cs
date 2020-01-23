// <copyright file="Commands.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Tello
{
    public enum Commands
    {
        EnterSdkMode,
        Takeoff,
        Land,
        Stop,
        StartVideo,
        StopVideo,
        EmergencyStop,
        Up,
        Down,
        Left,
        Right,
        Forward,
        Back,
        ClockwiseTurn,
        CounterClockwiseTurn,
        Flip,
        Go,
        Curve,
        SetSpeed,
        SetRemoteControl,
        SetWiFiPassword,

        // SetMissionPadOn,
        // SetMissionPadOff,
        // SetMissionPadDirection,
        SetStationMode,
        GetSpeed,
        GetBattery,
        GetTime,
        GetWIFISnr,
        GetSdkVersion,
        GetSerialNumber,
    }
}
