// <copyright file="IFlightController.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Threading.Tasks;
using Tello.Events;
using Tello.State;

namespace Tello
{
    public interface IFlightController
    {
        event EventHandler<ResponseReceivedArgs> ResponseReceived;

        event EventHandler<ExceptionThrownArgs> ExceptionThrown;

        event EventHandler<ConnectionStateChangedArgs> ConnectionStateChanged;

        event EventHandler<PositionChangedArgs> PositionChanged;

        event EventHandler<VideoStreamingStateChangedArgs> VideoStreamingStateChanged;

        InterogativeState InterogativeState { get; }

        Vector Position { get; }

        ITelloState State { get; }

        void UpdateState(object sender, StateChangedArgs e);

        /// <summary>
        /// establish a command link with Tello - validates network connection, connects to UDP, sends "command" to Tello.
        /// </summary>
        Task<bool> Connect();

        void Disconnect();

        /// <summary>
        /// auto take off to 20cm.
        /// </summary>
        void TakeOff();

        /// <summary>
        /// auto land.
        /// </summary>
        void Land();

        /// <summary>
        /// stops moving and hovers.
        /// </summary>
        Task Stop();

        /// <summary>
        /// stops motors immediately.
        /// </summary>
        Task EmergencyStop();

        /// <summary>
        /// begins H264 encoded video stream to port 11111, subscribe to FrameReady events to take advantage of video.
        /// </summary>
        void StartVideo();

        /// <summary>
        /// stops video stream.
        /// </summary>
        void StopVideo();

        /// <summary>
        ///
        /// </summary>
        /// <param name="cm">20 to 500.</param>
        void GoUp(int cm);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cm">20 to 500.</param>
        void GoDown(int cm);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cm">20 to 500.</param>
        void GoLeft(int cm);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cm">20 to 500.</param>
        void GoRight(int cm);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cm">20 to 500.</param>
        void GoForward(int cm);

        /// <summary>
        ///
        /// </summary>
        /// <param name="cm">20 to 500.</param>
        void GoBackward(int cm);

        /// <summary>
        ///
        /// </summary>
        /// <param name="degrees">1 to 360.</param>
        void TurnClockwise(int degress);

        /// <summary>
        ///
        /// </summary>
        /// <param name="degrees">1 to 360.</param>
        void TurnRight(int degress);

        /// <summary>
        ///
        /// </summary>
        /// <param name="degrees">1 to 360.</param>
        void TurnCounterClockwise(int degress);

        /// <summary>
        ///
        /// </summary>
        /// <param name="degrees">1 to 360.</param>
        void TurnLeft(int degress);

        /// <summary>
        ///
        /// </summary>
        /// <param name="FlipDirections">FlipDirections.Left, FlipDirections.Right, FlipDirections.Front, FlipDirections.Back.</param>
        void Flip(CardinalDirections direction);

        /// <summary>
        /// x, y and z values cannot be set between -20 and 20 simultaneously.
        /// </summary>
        /// <param name="x">-500 to 500.</param>
        /// <param name="y">-500 to 500.</param>
        /// <param name="z">-500 to 500.</param>
        /// <param name="speed">cm/s, 10 to 100.</param>
        void Go(int x, int y, int z, int speed);

        /// <summary>
        /// x, y and z values cannot be set between -20 and 20 simultaneously.
        /// </summary>
        /// <param name="x1">-500 to 500.</param>
        /// <param name="y1">-500 to 500.</param>
        /// <param name="z1">-500 to 500.</param>
        /// <param name="x2">-500 to 500.</param>
        /// <param name="y2">-500 to 500.</param>
        /// <param name="z2">-500 to 500.</param>
        /// <param name="speed">cm/s, 10 to 60.</param>
        void Curve(int x1, int y1, int z1, int x2, int y2, int z2, int speed);

        /// <summary>
        ///
        /// </summary>
        /// <param name="sides">3 to 15.</param>
        /// <param name="length">length of each side. 20 to 500 in cm.</param>
        /// <param name="speed">cm/s 10 to 100.</param>
        void FlyPolygon(int sides, int length, int speed, ClockDirections clockDirection);

        void SetHeight(int cm);

        /// <summary>
        ///
        /// </summary>
        /// <param name="speed">cm/s, 10 to 100.</param>
        void SetSpeed(int speed);

        /// <summary>
        ///
        /// </summary>
        /// <param name="leftRight">-100 to 100.</param>
        /// <param name="forwardBackward">-100 to 100.</param>
        /// <param name="upDown">-100 to 100.</param>
        /// <param name="yaw">-100 to 100.</param>
        void Set4ChannelRC(int leftRight, int forwardBackward, int upDown, int yaw);

        void SetWIFIPassword(string ssid, string password);

        void SetStationMode(string ssid, string password);

        /// <summary>
        /// get speed in cm/s, 10 to 100.
        /// </summary>
        void GetSpeed();

        /// <summary>
        /// get battery percentage remaining, 0% to 100%.
        /// </summary>
        void GetBattery();

        void GetTime();

        void GetWIFISNR();

        void GetSdkVersion();

        void GetSerialNumber();
    }
}
