using System;

namespace Tello.Messaging
{
    public class Response : IResponse
    {
        private Response() { }

        public static IResponse FromFailure(IRequest request, string errorMessage)
        {
            return new Response
            {
                IsSuccess = false,
                ErrorMessage = errorMessage,
                RequestId = request.Id,
            };
        }

        public static IResponse FromException(IRequest request, Exception ex, TimeSpan timeElapsed)
        {
            return new Response
            {
                IsSuccess = false,
                ErrorMessage = $"{ex.GetType().Name} - {ex.Message}",
                Exception = ex,
                TimeElapsed = timeElapsed,
                RequestId = request.Id,
            };
        }

        public static IResponse FromUdpReceiveResult(IRequest request, byte[] data, TimeSpan timeElapsed)
        {
            return new Response
            {
                IsSuccess = true,
                ErrorMessage = "ok",
                Data = data,
                TimeElapsed = timeElapsed,
                RequestId = request.Id,
            };
        }

        public byte[] Data { get; private set; }
        public TimeSpan TimeElapsed { get; private set; }

        public bool IsSuccess { get; private set; } = false;
        public string ErrorMessage { get; private set; }
        public Exception Exception { get; private set; }

        public Guid RequestId { get; private set; }
}
}
