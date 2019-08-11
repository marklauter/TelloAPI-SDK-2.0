using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Messenger.Simulator
{
    public sealed class SimTransceiver : Transceiver, ITransceiver
    {
        private readonly IDroneMessageHandler _drone;

        public SimTransceiver(IDroneMessageHandler drone)
        {
            _drone = drone ?? throw new ArgumentNullException(nameof(drone));
        }

        protected override async Task<IResponse> Send(IRequest request)
        {
            var stopwatch = Stopwatch.StartNew();

            return request.NoWait
                ? new Response(request, TimeSpan.Zero, true)
                : new Response(request, await _drone.Invoke(request.Data), stopwatch.Elapsed);
        }

        public override void Dispose() { }
    }
}

