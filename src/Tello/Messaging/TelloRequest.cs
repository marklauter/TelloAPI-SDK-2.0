using Messenger;
using System;
using System.Text;

namespace Tello.Messaging
{
    public sealed class TelloRequest : Request<string>
    {
        public TelloRequest(Command command) 
            : base(
                  (string)command, 
                  (TimeSpan)command)
        {
        }

        protected override byte[] Serialize(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
