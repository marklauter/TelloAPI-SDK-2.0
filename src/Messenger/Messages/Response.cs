using System;

namespace Messenger
{
    public abstract class Response<T> : IResponse<T>
    {
        public Response(IRequest request, Exception exception, TimeSpan timeTaken)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));
            Exception = exception ?? throw new ArgumentNullException(nameof(exception));
            StatusMessage = $"{exception.GetType().Name} - {exception.Message}";
            TimeTaken = timeTaken;
            Success = false;
        }

        public Response(IRequest request, byte[] data, TimeSpan timeTaken)
        {
            Request = request ?? throw new ArgumentNullException(nameof(request));

            if (data == null)
            {
                throw new ArgumentNullException(nameof(data));
            }

            if (data.Length == 0)
            {
                throw new ArgumentOutOfRangeException(nameof(data));
            }

            Data = data;
            Message = Deserialize(data);

            TimeTaken = timeTaken;
            Success = true;
        }

        protected abstract T Deserialize(byte[] data);

        public IRequest Request {get;}

        public TimeSpan TimeTaken {get;}

        public bool Success {get;}

        public string StatusMessage {get;}

        public Exception Exception {get;}

        public T Message {get;}

        public byte[] Data {get;}

        public DateTime Timestamp {get;}

        public Guid Id {get;}
    }
}
