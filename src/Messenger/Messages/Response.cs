using System;

namespace Tello.Messaging
{
    public class Response : IResponse
    {
        private Response() { }

        public static IResponse FromException(IMessage request, Exception ex, TimeSpan timeElapsed)
        {
            return new Response
            {
                Success = false,
                Message = $"{ex.GetType().Name} - {ex.Message}",
                Exception = ex,
                Interval = timeElapsed,
                RequestId = request.Id,
            };
        }

        public static IResponse FromData(IMessage request, byte[] data, TimeSpan timeElapsed)
        {
            return new Response
            {
                Success = true,
                Data = data,
                Interval = timeElapsed,
                RequestId = request.Id,
            };
        }

        public byte[] Data { get; private set; }
        public TimeSpan Interval { get; private set; }

        public bool Success { get; private set; } = false;
        public string Message { get; private set; }
        public Exception Exception { get; private set; }

        public Guid RequestId { get; private set; }
}
}
