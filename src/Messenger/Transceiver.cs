// <copyright file="Transceiver.cs" company="Mark Lauter">
// Copyright (c) Mark Lauter. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.
// </copyright>

using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Messenger
{
    public abstract class Transceiver : ITransceiver
    {
        public async Task<IResponse> SendAsync(IRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                return await this.Send(request);
            }
            catch (Exception ex)
            {
                return new Response(request, ex, stopwatch.Elapsed);
            }
        }

        protected abstract Task<IResponse> Send(IRequest request);

        public abstract void Dispose();
    }
}