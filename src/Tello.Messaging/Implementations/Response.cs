using System;

namespace Tello.Messaging
{
    public class Response : IResponse
    {
        private Response() { }

        public static IResponse FromFailure(string errorMessage)
        {
            return new Response
            {
                IsSuccess = false,
                ErrorMessage = errorMessage
            };
        }

        public static IResponse FromException(Exception ex, TimeSpan timeElapsed)
        {
            return new Response
            {
                IsSuccess = false,
                ErrorMessage = $"{ex.GetType().Name} - {ex.Message}",
                Exception = ex,
                TimeElapsed = timeElapsed
            };
        }

        public static IResponse FromUdpReceiveResult(byte[] data, TimeSpan timeElapsed)
        {
            return new Response
            {
                IsSuccess = true,
                ErrorMessage = "ok",
                Data = data,
                TimeElapsed = timeElapsed
            };
        }

        public byte[] Data { get; private set; }
        public TimeSpan TimeElapsed { get; private set; }

        public bool IsSuccess { get; private set; } = false;
        public string ErrorMessage { get; private set; }
        public Exception Exception { get; private set; }
    }
}
