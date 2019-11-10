// <copyright file="SimTransceiver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public sealed class SimTransceiver : Transceiver, ITransceiver
    {
        private readonly IDroneMessageHandler drone;

        public SimTransceiver(IDroneMessageHandler drone)
        {
            this.drone = drone ?? throw new ArgumentNullException(nameof(drone));
        }

        protected override async Task<IResponse> Send(IRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            return request.NoWait
                ? new Response(request, TimeSpan.Zero, true)
                : new Response(request, await this.drone.Invoke(request.Data), stopwatch.Elapsed);
        }

        public override void Dispose()
        {
        }
    }
}
