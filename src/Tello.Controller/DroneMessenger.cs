// <copyright file="DroneMessenger.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using Messenger;
using Tello.Events;

namespace Tello.Controller
{
    public sealed class DroneMessenger
    {
        private readonly ITransceiver transceiver;
        private readonly IReceiver stateReceiver;
        private readonly IReceiver videoReceiver;

        public DroneMessenger(
            ITransceiver transceiver,
            IReceiver stateReceiver,
            IReceiver videoReceiver)
        {
            this.transceiver = transceiver ?? throw new ArgumentNullException(nameof(transceiver));
            this.stateReceiver = stateReceiver ?? throw new ArgumentNullException(nameof(stateReceiver));
            this.videoReceiver = videoReceiver ?? throw new ArgumentNullException(nameof(videoReceiver));

            this.Controller = new FlightController(this.transceiver);

            this.Controller.ConnectionStateChanged +=
                (object sender, ConnectionStateChangedArgs e) =>
                {
                    if (e.IsConnected)
                    {
                        this.StartLisenters();
                    }
                    else
                    {
                        this.StopListeners();
                    }
                };

            this.StateObserver = new StateObserver(this.stateReceiver);
            this.StateObserver.StateChanged += this.Controller.UpdateState;
            this.Controller.PositionChanged += this.StateObserver.UpdatePosition;

            this.VideoObserver = new VideoObserver(this.videoReceiver);
        }

        #region Listeners

        private void StartLisenters()
        {
            this.stateReceiver.Start();
            this.videoReceiver.Start();
        }

        private void StopListeners()
        {
            this.stateReceiver.Stop();
            this.videoReceiver.Stop();
        }

        #endregion

        public IFlightController Controller { get; }

        public IStateObserver StateObserver { get; }

        public IVideoObserver VideoObserver { get; }
    }
}
