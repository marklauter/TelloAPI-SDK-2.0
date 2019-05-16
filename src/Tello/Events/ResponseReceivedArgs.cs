using System;
using Tello.Messaging;

namespace Tello.Events
{
    public sealed class ResponseReceivedArgs : EventArgs
    {
        public ResponseReceivedArgs(TelloResponse response)
        {
            Response = response ?? throw new ArgumentNullException(nameof(response));
        }

        public TelloResponse Response { get; }
    }
}
