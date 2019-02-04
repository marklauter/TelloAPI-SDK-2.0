using System;

namespace Tello.Messaging
{
    public class Response : IResponse
    {
        private Response() { }

        public static IResponse FromException(IRequest request, Exception ex, TimeSpan timeElapsed)
        {
            return new Response
            {
                Successful = false,
                ErrorMessage = $"{ex.GetType().Name} - {ex.Message}",
                Exception = ex,
                TimeElapsed = timeElapsed,
                RequestId = request.Id,
            };
        }

        public static IResponse FromData(IRequest request, byte[] data, TimeSpan timeElapsed)
        {
            return new Response
            {
                Successful = true,
                Data = data,
                TimeElapsed = timeElapsed,
                RequestId = request.Id,
            };
        }

        public byte[] Data { get; private set; }
        public TimeSpan TimeElapsed { get; private set; }

        public bool Successful { get; private set; } = false;
        public string ErrorMessage { get; private set; }
        public Exception Exception { get; private set; }

        public Guid RequestId { get; private set; }
}
}
