using System;
using System.Text;
using Tello;

namespace Messenger.Tello
{
    public sealed class TelloRequest : Request<string>
    {
        public TelloRequest(Command command) 
            : base((string)command, (TimeSpan)command)
        {
        }

        protected override byte[] Serialize(string message)
        {
            return Encoding.UTF8.GetBytes(message);
        }
    }
}
