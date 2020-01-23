// <copyright file="TelloControllerViewModel.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using Repository;
using Tello.App.MvvM;
using Tello.Entities;
using Tello.Entities.Sqlite;
using Tello.State;

namespace Tello.App.ViewModels
{
    public class TelloControllerViewModel : ViewModel
    {
        private readonly IFlightController controller;
        private readonly IRepository repository;
        private readonly ISession session;

        public TelloControllerViewModel(
            IUIDispatcher dispatcher,
            IUINotifier notifier,
            IFlightController controller,
            IRepository repository,
            ISession session)
            : base(dispatcher, notifier)
        {
            this.controller = controller ?? throw new ArgumentNullException(nameof(controller));
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
            this.session = session ?? throw new ArgumentNullException(nameof(session));
        }

        protected override void OnOpen(OpenEventArgs args)
        {
            this.controller.ConnectionStateChanged += this.ConnectionStateChanged;
            this.controller.ExceptionThrown += this.ExceptionThrown;
            this.controller.PositionChanged += this.PositionChanged;
            this.controller.ResponseReceived += this.ResponseReceived;
            this.controller.VideoStreamingStateChanged += this.Controller_VideoStreamingStateChanged;
        }

        protected override void OnClosing(ClosingEventArgs args)
        {
            this.controller.ConnectionStateChanged -= this.ConnectionStateChanged;
            this.controller.ExceptionThrown -= this.ExceptionThrown;
            this.controller.PositionChanged -= this.PositionChanged;
            this.controller.ResponseReceived -= this.ResponseReceived;
        }

        private bool isVideoStreaming = false;

        public bool IsVideoStreaming
        {
            get => this.isVideoStreaming;
            set => this.SetProperty(ref this.isVideoStreaming, value);
        }

        private void Controller_VideoStreamingStateChanged(object sender, Events.VideoStreamingStateChangedArgs e)
        {
            this.Dispatcher.Invoke(
                (isStreaming) =>
                this.IsVideoStreaming = (bool)isStreaming,
                e.IsStreaming);

            var message = $"{DateTime.Now.TimeOfDay} - streaming ? {e.IsStreaming}";
            Debug.WriteLine(message);
            this.Dispatcher.Invoke(
                (msg) =>
            {
                this.ControlLog.Insert(0, msg as string);
                if (this.ControlLog.Count > 500)
                {
                    this.ControlLog.RemoveAt(this.ControlLog.Count - 1);
                }
            },
                message);
        }

        private void ResponseReceived(object sender, Events.ResponseReceivedArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - '{(Command)e.Response.Request.Data}' completed with response '{e.Response.Message}' in {(int)e.Response.TimeTaken.TotalMilliseconds}ms";
            Debug.WriteLine(message);
            this.Dispatcher.Invoke(
                (msg) =>
            {
                this.ControlLog.Insert(0, msg as string);
                if (this.ControlLog.Count > 500)
                {
                    this.ControlLog.RemoveAt(this.ControlLog.Count - 1);
                }
            },
                message);

            var group = this.repository.NewEntity<ObservationGroup>(this.session);
            this.repository.Insert(new ResponseObservation(group, e.Response));
        }

        private void PositionChanged(object sender, Events.PositionChangedArgs e)
        {
            this.Dispatcher.Invoke(
                (position) => this.Position = position as Vector,
                e.Position);

            var message = $"{DateTime.Now.TimeOfDay} - new position {e.Position}";
            Debug.WriteLine(message);
            this.Dispatcher.Invoke(
                (msg) =>
                {
                    this.ControlLog.Insert(0, msg as string);
                    if (this.ControlLog.Count > 500)
                    {
                        this.ControlLog.RemoveAt(this.ControlLog.Count - 1);
                    }
                },
                message);
        }

        private void ConnectionStateChanged(object sender, Events.ConnectionStateChangedArgs e)
        {
            this.Dispatcher.Invoke(
                (connected) => this.IsConnected = (bool)connected,
                e.IsConnected);

            var message = $"{DateTime.Now.TimeOfDay} - connected ? {e.IsConnected}";
            Debug.WriteLine(message);
            this.Dispatcher.Invoke(
                (msg) =>
                {
                    this.ControlLog.Insert(0, msg as string);
                    if (this.ControlLog.Count > 500)
                    {
                        this.ControlLog.RemoveAt(this.ControlLog.Count - 1);
                    }
                },
                message);
        }

        private void ExceptionThrown(object sender, Events.ExceptionThrownArgs e)
        {
            var message = $"{DateTime.Now.TimeOfDay} - Controller failed with exception '{e.Exception.GetType().FullName}' - {e.Exception.Message} at {e.Exception.StackTrace}";
            Debug.WriteLine(message);
            this.Dispatcher.Invoke((msg) => this.ControlLog.Insert(0, msg as string), message);
        }

        public ObservableCollection<string> ControlLog { get; } = new ObservableCollection<string>();

        internal void ClearDatabase()
        {
            if (this.repository != null)
            {
                this.repository.Delete<ResponseObservation>();
            }
        }

        private bool isConnected = false;

        public bool IsConnected
        {
            get => this.isConnected;
            set => this.SetProperty(ref this.isConnected, value);
        }

        private Vector position = new Vector();

        public Vector Position
        {
            get => this.position;
            set => this.SetProperty(ref this.position, value);
        }

#pragma warning disable IDE1006 // Naming Styles
        private IInputCommand clearDatabaseCommand;

        public IInputCommand ClearDatabaseCommand => this.clearDatabaseCommand = this.clearDatabaseCommand ?? new InputCommand(this.ClearDatabase);

        private IInputCommand enterSdkModeCommand;

        public IInputCommand EnterSdkModeCommand => this.enterSdkModeCommand = this.enterSdkModeCommand ?? new AsyncInputCommand(this.controller.Connect);

        private IInputCommand disconnectCommand;

        public IInputCommand DisconnectCommand => this.disconnectCommand = this.disconnectCommand ?? new InputCommand(this.controller.Disconnect);

        private IInputCommand takeOffCommand;

        public IInputCommand TakeOffCommand => this.takeOffCommand = this.takeOffCommand ?? new InputCommand(this.controller.TakeOff);

        private IInputCommand landCommand;

        public IInputCommand LandCommand => this.landCommand = this.landCommand ?? new InputCommand(this.controller.Land);

        private IInputCommand stopCommand;

        public IInputCommand StopCommand => this.stopCommand = this.stopCommand ?? new AsyncInputCommand(this.controller.Stop);

        private IInputCommand emergencyStopCommand;

        public IInputCommand EmergencyStopCommand => this.emergencyStopCommand = this.emergencyStopCommand ?? new AsyncInputCommand(this.controller.EmergencyStop);

        private IInputCommand startVideoCommand;

        public IInputCommand StartVideoCommand => this.startVideoCommand = this.startVideoCommand ?? new InputCommand(this.controller.StartVideo);

        private IInputCommand stopVideoCommand;

        public IInputCommand StopVideoCommand => this.stopVideoCommand = this.stopVideoCommand ?? new InputCommand(this.controller.StopVideo);

        private IInputCommand<int> goUpCommand;

        public IInputCommand<int> GoUpCommand => this.goUpCommand = this.goUpCommand ?? new InputCommand<int>(this.controller.GoUp);

        private IInputCommand<int> goDownCommand;

        public IInputCommand<int> GoDownCommand => this.goDownCommand = this.goDownCommand ?? new InputCommand<int>(this.controller.GoDown);

        private IInputCommand<int> goLeftCommand;

        public IInputCommand<int> GoLeftCommand => this.goLeftCommand = this.goLeftCommand ?? new InputCommand<int>(this.controller.GoLeft);

        private IInputCommand<int> goRightCommand;

        public IInputCommand<int> GoRightCommand => this.goRightCommand = this.goRightCommand ?? new InputCommand<int>(this.controller.GoRight);

        private IInputCommand<int> goForwardCommand;

        public IInputCommand<int> GoForwardCommand => this.goForwardCommand = this.goForwardCommand ?? new InputCommand<int>(this.controller.GoForward);

        private IInputCommand<int> goBackwardCommand;

        public IInputCommand<int> GoBackwardCommand => this.goBackwardCommand = this.goBackwardCommand ?? new InputCommand<int>(this.controller.GoBackward);

        private IInputCommand<int> turnClockwiseCommand;

        public IInputCommand<int> TurnClockwiseCommand => this.turnClockwiseCommand = this.turnClockwiseCommand ?? new InputCommand<int>(this.controller.TurnClockwise);

        private IInputCommand<int> turnRightCommand;

        public IInputCommand<int> TurnRightCommand => this.turnRightCommand = this.turnRightCommand ?? new InputCommand<int>(this.controller.TurnRight);

        private IInputCommand<int> turnCounterClockwiseCommand;

        public IInputCommand<int> TurnCounterClockwiseCommand => this.turnCounterClockwiseCommand = this.turnCounterClockwiseCommand ?? new InputCommand<int>(this.controller.TurnCounterClockwise);

        private IInputCommand<int> turnLeftCommand;

        public IInputCommand<int> TurnLeftCommand => this.turnLeftCommand = this.turnLeftCommand ?? new InputCommand<int>(this.controller.TurnLeft);

        private IInputCommand<CardinalDirections> flipCommand;

        public IInputCommand<CardinalDirections> FlipCommand => this.flipCommand = this.flipCommand ?? new InputCommand<CardinalDirections>(this.controller.Flip);

        private IInputCommand<Tuple<int, int, int, int>> goCommand;

        public IInputCommand<Tuple<int, int, int, int>> GoCommand => this.goCommand = this.goCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => this.controller.Go(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));

        private IInputCommand<Tuple<int, int, int, int, int, int, int>> curveCommand;

        public IInputCommand<Tuple<int, int, int, int, int, int, int>> CommandNameCommand => this.curveCommand = this.curveCommand ?? new InputCommand<Tuple<int, int, int, int, int, int, int>>((tuple) => this.controller.Curve(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4, tuple.Item5, tuple.Item6, tuple.Item7));

        // sides, length, speed, clock, do land
        public Tuple<int, int, int, ClockDirections> FlyPolygonCommandParams { get; } = new Tuple<int, int, int, ClockDirections>(3, 100, 50, ClockDirections.Clockwise);

        private IInputCommand<Tuple<int, int, int, ClockDirections>> flyPolygonCommand;

        public IInputCommand<Tuple<int, int, int, ClockDirections>> FlyPolygonCommand => this.flyPolygonCommand = this.flyPolygonCommand ?? new InputCommand<Tuple<int, int, int, ClockDirections>>((tuple) => this.controller.FlyPolygon(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));

        private IInputCommand<int> setSpeedCommand;

        public IInputCommand<int> SetSpeedCommand => this.setSpeedCommand = this.setSpeedCommand ?? new InputCommand<int>(this.controller.SetSpeed);

        private IInputCommand<Tuple<int, int, int, int>> set4ChannelRCCommand;

        public IInputCommand<Tuple<int, int, int, int>> Set4ChannelRCCommand => this.set4ChannelRCCommand = this.set4ChannelRCCommand ?? new InputCommand<Tuple<int, int, int, int>>((tuple) => this.controller.Set4ChannelRC(tuple.Item1, tuple.Item2, tuple.Item3, tuple.Item4));

        private IInputCommand<Tuple<string, string>> setWIFIPasswordCommand;

        public IInputCommand<Tuple<string, string>> SetWIFIPassowrdCommand => this.setWIFIPasswordCommand = this.setWIFIPasswordCommand ?? new InputCommand<Tuple<string, string>>((tuple) => this.controller.SetWIFIPassword(tuple.Item1, tuple.Item2));

        private IInputCommand<Tuple<string, string>> setStationModeCommand;

        public IInputCommand<Tuple<string, string>> SetStationModeCommand => this.setStationModeCommand = this.setStationModeCommand ?? new InputCommand<Tuple<string, string>>((tuple) => this.controller.SetStationMode(tuple.Item1, tuple.Item2));

        private IInputCommand getSpeedCommand;

        public IInputCommand GetSpeedCommand => this.getSpeedCommand = this.getSpeedCommand ?? new InputCommand(this.controller.GetSpeed);

        private IInputCommand getBatteryCommand;

        public IInputCommand GetBatteryCommand => this.getBatteryCommand = this.getBatteryCommand ?? new InputCommand(this.controller.GetBattery);

        private IInputCommand getTimeCommand;

        public IInputCommand GetTimeCommand => this.getTimeCommand = this.getTimeCommand ?? new InputCommand(this.controller.GetTime);

        private IInputCommand getWIFISNRCommand;

        public IInputCommand GetWIFISNRCommand => this.getWIFISNRCommand = this.getWIFISNRCommand ?? new InputCommand(this.controller.GetWIFISNR);

        private IInputCommand getSdkVersionCommand;

        public IInputCommand GetSdkVersionCommand => this.getSdkVersionCommand = this.getSdkVersionCommand ?? new InputCommand(this.controller.GetSdkVersion);

        private IInputCommand getSerialNumberCommand;

        public IInputCommand GetSerialNumberCommand => this.getSerialNumberCommand = this.getSerialNumberCommand ?? new InputCommand(this.controller.GetSerialNumber);
#pragma warning restore IDE1006 // Naming Styles

    }
}
