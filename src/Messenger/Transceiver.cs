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
                return await Send(request);
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