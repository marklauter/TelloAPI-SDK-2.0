// <copyright file="JoyStickFlightTest.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Messenger;
using Repository;
using Tello.Controller;
using Tello.Entities.Sqlite;

namespace Tello.Demo
{
    public sealed class JoyStickFlightTest : IFlightTest
    {
        private readonly DroneMessenger tello;
        private readonly IRepository repository;
        private readonly Session session;
        private bool canMove = false;

        public JoyStickFlightTest(
            IRepository repository,
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
        {
            this.tello = new DroneMessenger(transceiver, stateReceiver, videoReceiver);
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));

            this.tello.Controller.ConnectionStateChanged += this.Controller_ConnectionStateChanged;
            this.tello.Controller.ExceptionThrown += this.Controller_ExceptionThrown;
            this.tello.Controller.ResponseReceived += this.Controller_ResponseReceived;

            this.tello.StateObserver.StateChanged += this.StateObserver_StateChanged;
            this.tello.VideoObserver.VideoSampleReady += this.VideoObserver_VideoSampleReady;

            this.repository.CreateCatalog<Session>();
            this.repository.CreateCatalog<ObservationGroup>();
            this.repository.CreateCatalog<StateObservation>();
            this.repository.CreateCatalog<AirSpeedObservation>();
            this.repository.CreateCatalog<AttitudeObservation>();
            this.repository.CreateCatalog<BatteryObservation>();
            this.repository.CreateCatalog<HobbsMeterObservation>();
            this.repository.CreateCatalog<PositionObservation>();
            this.repository.CreateCatalog<ResponseObservation>();

            this.session = this.repository.NewEntity<Session>();
        }

        public async Task Invoke()
        {
            Console.WriteLine("Make sure Tello is powered up and you're connected to the network before continuing.");
            Console.WriteLine("press any key when ready to start flight test...");
            Console.ReadKey(true);

            Log.WriteLine("> enter sdk mode");
            if (await this.tello.Controller.Connect())
            {
                Console.WriteLine("Remember to turn Tello off to keep it from overheating.");
                Console.WriteLine("press any key when ready to end program...");
            }
            else
            {
                Log.WriteLine("connection failed - program will be terminated");
            }

            Console.ReadKey(true);

            if (this.videoFile != null)
            {
                this.videoFile.Close();
            }

            this.repository.Dispose();
        }

        private void Continue()
        {
            // Log.WriteLine("> start video");
            // _tello.Controller.StartVideo();
            Log.WriteLine("> take off");
            this.tello.Controller.TakeOff();

            var spinWait = default(SpinWait);
            while (!this.canMove)
            {
                spinWait.SpinOnce();
            }

            Console.WriteLine("READY FOR FLIGHT!");
            Console.WriteLine();
            Console.Beep(700, 500);

            Console.WriteLine("W - forward");
            Console.WriteLine("S - backward");
            Console.WriteLine("A - left");
            Console.WriteLine("D - right");
            Console.WriteLine("Up Arrow - up");
            Console.WriteLine("Down Arrow - down");
            Console.WriteLine("Right Arrow - turn right");
            Console.WriteLine("Left Arrow - turn left");
            Console.WriteLine("ESC to exit");

            var joystick = new JoyStick();
            var scanstate = JoyStick.ScanStates.Unchanged;
            do
            {
                if (scanstate == JoyStick.ScanStates.Changed)
                {
                    this.tello.Controller.Set4ChannelRC(joystick.XInput, joystick.ZInput, joystick.YInput, joystick.RInput);
                }

                scanstate = joystick.Scan();
            }
            while (scanstate != JoyStick.ScanStates.Exit);

            Log.WriteLine("> land");
            this.tello.Controller.Land();
        }

        #region event handlers
        private void Controller_ResponseReceived(object sender, Events.ResponseReceivedArgs e)
        {
            if (!this.canMove
                && (Command)e.Response.Request.Data == Commands.Takeoff
                && e.Response.Success && e.Response.Message == Responses.Ok.ToString().ToLowerInvariant())
            {
                this.canMove = true;
            }

            // Log.WriteLine($"{(Command)e.Response.Request.Data} returned '{e.Response.Message}' in {(int)e.Response.TimeTaken.TotalMilliseconds}ms", ConsoleColor.Cyan);
            var group = this.repository.NewEntity<ObservationGroup>(this.session);
            this.repository.Insert(new ResponseObservation(group, e.Response));
        }

        private long videoLength = 0;
        private FileStream videoFile = null;

        private void VideoObserver_VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            this.videoFile = this.videoFile ?? File.OpenWrite($"tello.video.{this.session.Id}.h264");
            this.videoFile.Write(e.Message.Data, 0, e.Message.Data.Length);

            this.videoLength = this.videoLength < Int64.MaxValue
                ? this.videoLength + e.Message.Data.Length
                : 0;
        }

        private void StateObserver_StateChanged(object sender, Events.StateChangedArgs e)
        {
            var group = this.repository.NewEntity<ObservationGroup>(this.session);
            this.repository.Insert(new StateObservation(group, e.State));
            this.repository.Insert(new AirSpeedObservation(group, e.State));
            this.repository.Insert(new AttitudeObservation(group, e.State));
            this.repository.Insert(new BatteryObservation(group, e.State));
            this.repository.Insert(new HobbsMeterObservation(group, e.State));
            this.repository.Insert(new PositionObservation(group, e.State));
        }

        private void Controller_ExceptionThrown(object sender, Events.ExceptionThrownArgs e)
        {
            Log.WriteLine($"Exception {e.Exception.InnerException.GetType()} with message - {e.Exception.InnerException.Message}", ConsoleColor.Red, false);
            Log.WriteLine("| Stack trace", ConsoleColor.Red, false);
            Log.WriteLine($"| {e.Exception.InnerException.StackTrace}", ConsoleColor.Red);
        }

        private void Controller_ConnectionStateChanged(object sender, Events.ConnectionStateChangedArgs e)
        {
            Log.WriteLine($"Connection State: {e.IsConnected}");
            if (e.IsConnected)
            {
                this.Continue();
            }
        }
        #endregion
    }
}
