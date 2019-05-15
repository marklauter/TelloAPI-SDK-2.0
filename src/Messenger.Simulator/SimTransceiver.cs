using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Tello.Simulator;

namespace Messenger.Simulator
{
    public sealed class SimTransceiver : Transceiver, ITransceiver
    {
        private readonly Drone _droneSim;

        public SimTransceiver(Drone droneSim)
        {
            _droneSim = droneSim ?? throw new ArgumentNullException(nameof(droneSim));
        }

        protected override async Task<IResponse> Send(IRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            return new Response(
                request,
                await _droneSim.Invoke(request.Data),
                stopwatch.Elapsed);
        }

        public override void Dispose() { }
    }
}

