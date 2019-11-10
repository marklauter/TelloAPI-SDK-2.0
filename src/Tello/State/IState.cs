// <copyright file="IState.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;

namespace Tello.State
{
    public interface IState
    {
        DateTime Timestamp { get; }
    }

    public interface ITelloState : IState
    {
        bool MissionPadDetected { get; }

        int MissionPadId { get; }

        int MissionPadX { get; }

        int MissionPadY { get; }

        int MissionPadZ { get; }

        int MissionPadPitch { get; }

        int MissionPadRoll { get; }

        int MissionPadYaw { get; }

        int Pitch { get; }

        int Roll { get; }

        int Yaw { get; }

        int SpeedX { get; }

        int SpeedY { get; }

        int SpeedZ { get; }

        int TemperatureLowC { get; }

        int TemperatureHighC { get; }

        int DistanceTraversedInCm { get; }

        int HeightInCm { get; }

        int BatteryPercent { get; }

        int MotorTimeInSeconds { get; }

        double BarometerInCm { get; }

        double AccelerationX { get; }

        double AccelerationY { get; }

        double AccelerationZ { get; }

        string Data { get; }

        IPosition Position { get; }

        IAttitude Attitude { get; }

        IAirSpeed AirSpeed { get; }

        IBattery Battery { get; }

        IHobbsMeter HobbsMeter { get; }
    }

    public interface IPosition : IState
    {
        int AltitudeAGLInCm { get; }

        double BarometricPressueInCm { get; }

        int Heading { get; }

        double X { get; }

        double Y { get; }
    }

    public interface IAttitude : IState
    {
        int Pitch { get; }

        int Roll { get; }

        int Yaw { get; }
    }

    public interface IAirSpeed : IState
    {
        /// <summary>
        /// lateral airspeed.
        /// </summary>
        int SpeedX { get; }

        /// <summary>
        /// forward/backward airspeed.
        /// </summary>
        int SpeedY { get; }

        /// <summary>
        /// vertical airspeed.
        /// </summary>
        int SpeedZ { get; }

        double AccelerationX { get; }

        double AccelerationY { get; }

        double AccelerationZ { get; }
    }

    public interface IBattery : IState
    {
        int TemperatureLowC { get; }

        int TemperatureHighC { get; }

        int PercentRemaining { get; }
    }

    public interface IHobbsMeter : IState
    {
        int DistanceTraversedInCm { get; }

        int MotorTimeInSeconds { get; }

        double FlightTimeRemainingInMinutes { get; }
    }
}
