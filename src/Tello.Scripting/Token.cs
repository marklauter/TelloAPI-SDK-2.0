using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Tello.Messaging;

namespace Tello.Scripting
{
    public class Token
    {
        public int Order { get; internal set; }

        public object[] Args { get; set; }

        [JsonConverter(typeof(StringEnumConverter))]
        public Commands Command { get; set; }

        public string Id { get; set; }
    }
}
