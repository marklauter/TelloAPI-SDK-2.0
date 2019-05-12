using System;

namespace Messenger
{
    public class Response : IResponse
    {
        protected Response(IResponse response)
        {
            if (response == null)
            {
                throw new ArgumentNullException(nameof(response));
            }

            Id = response.Id;
            Timestamp = response.Timestamp;
            Data = response.Data;

            Request = response.Request;
            Exception = response.Exception;
            StatusMessage = response.StatusMessage;
            TimeTaken = response.TimeTaken;
            Success = response.Success;
        }

        private Response(IRequest request, TimeSpan timeTaken, bool success)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            TimeTaken = timeTaken;
            Success = success;
            Timestamp = DateTime.UtcNow;
            Id = Guid.NewGuid();
        }

        public Response(IRequest request, Exception exception, TimeSpan timeTaken) : this(request, timeTaken, false)
        {
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            StatusMessage = $"{exception.GetType().Name} - {exception.Message}";
        }

        public Response(IRequest request, byte[] data, TimeSpan timeTaken) : this(request, timeTaken, true)
        {
            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            Data = data;
        }

        public IRequest Request { get; }

        public TimeSpan TimeTaken { get; }

        public bool Success { get; }

        public string StatusMessage { get; }

        public Exception Exception { get; }

        public byte[] Data { get; }

        public DateTime Timestamp { get; }

        public Guid Id { get; }
    }

    public abstract class Response<T> : Response, IResponse<T>
    {
        public Response(IResponse response) 
            : base(response)
        {
            Message = Deserialize(Data);
        }

        public Response(IRequest request, Exception exception, TimeSpan timeTaken) 
            : base(request, exception, timeTaken)
        {
            Message = Deserialize(Data);
        }

        public Response(IRequest request, byte[] data, TimeSpan timeTaken) 
            : base(request, data, timeTaken)
        {
            Message = Deserialize(Data);
        }
        
        protected abstract T Deserialize(byte[] data);

        public T Message { get; }
    }
}
