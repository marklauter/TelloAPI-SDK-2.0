// <copyright file="CommandFlightTest.cs" company="Mark Lauter">
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
    public sealed class CommandFlightTest : IFlightTest
    {
        private readonly DroneMessenger tello;
        private readonly IRepository repository;
        private readonly Session session;
        private bool canMove = false;

        public CommandFlightTest(
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
            Console.ReadKey(false);

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

            Log.WriteLine("> go forward");
            this.tello.Controller.GoForward(50);

            Log.WriteLine("> go backward");
            this.tello.Controller.GoBackward(50);

            Log.WriteLine("> fly polygon");
            this.tello.Controller.FlyPolygon(4, 100, 50, ClockDirections.Clockwise);

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

            Log.WriteLine($"{(Command)e.Response.Request.Data} returned '{e.Response.Message}' in {(int)e.Response.TimeTaken.TotalMilliseconds}ms", ConsoleColor.Cyan);
            Log.WriteLine($"Estimated Position: {this.tello.Controller.Position}", ConsoleColor.Blue);

            var group = this.repository.NewEntity<ObservationGroup>(this.session);
            this.repository.Insert(new ResponseObservation(group, e.Response));
        }

        private int videoCount = 0;
        private long videoLength = 0;
        private FileStream videoFile = null;

        private void VideoObserver_VideoSampleReady(object sender, Events.VideoSampleReadyArgs e)
        {
            this.videoFile = this.videoFile ?? File.OpenWrite($"tello.video.{this.session.Id}.h264");
            this.videoFile.Write(e.Message.Data, 0, e.Message.Data.Length);

            this.videoLength = this.videoLength < Int64.MaxValue
                ? this.videoLength + e.Message.Data.Length
                : 0;

            // video reporting interval is 30hz, so 150 should be once every 5 seconds
            if (this.videoCount % 150 == 0)
            {
                Log.WriteLine($"video segment size {e.Message.Data.Length}b @ {e.Message.Timestamp.ToString("o")}", ConsoleColor.Magenta, false);
                Log.WriteLine($"ttl video size {this.videoLength}b, segment count {this.videoCount + 1}", ConsoleColor.Magenta);
            }

            this.videoCount = this.videoCount < Int32.MaxValue
                ? this.videoCount + 1
                : 0;
        }

        private int stateCount = 0;

        private void StateObserver_StateChanged(object sender, Events.StateChangedArgs e)
        {
            // state reporting interval is 5hz, so 25 should be once every 5 seconds, 50 once every 10 seconds
            if (this.stateCount % 50 == 0)
            {
                Log.WriteLine($"state: {e.State}", ConsoleColor.Yellow);
            }

            this.stateCount = this.stateCount < Int32.MaxValue
                ? this.stateCount + 1
                : 0;

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
