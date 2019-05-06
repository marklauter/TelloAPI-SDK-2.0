using Sumo.Retry;
using Sumo.Retry.Policies;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace Messenger
{
    public abstract class Transceiver : ITransceiver
    {
        public Transceiver(RetryPolicy retryPolicy)
        {
            _retryPolicy = retryPolicy ?? throw new ArgumentNullException(nameof(retryPolicy));
        }

        private readonly RetryPolicy _retryPolicy;

        public async Task<IResponse> SendAsync(IRequest request)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                return await WithRetry.InvokeAsync(
                   _retryPolicy,
                   async () => await Send(request));
            }
            catch(Exception ex)
            {
                return new Response(request, ex, stopwatch.Elapsed);
            }
        }

        protected abstract Task<IResponse> Send(IRequest request);
    }
}