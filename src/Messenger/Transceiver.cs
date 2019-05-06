using Sumo.Retry;
using Sumo.Retry.Policies;
using System;
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

        public async Task<IResponse<T>> SendAsync<T>(IRequest request)
        {
            return await WithRetry.InvokeAsync(
                _retryPolicy,
                async () => await Send<T>(request));
        }

        protected abstract Task<IResponse<T>> Send<T>(IRequest request);
    }
}