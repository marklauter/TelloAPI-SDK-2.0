namespace Tello.Messaging
{
    public class Notification : INotification
    {
        private Notification(byte[] data) { Data = data; }

        public static INotification FromData(byte[] data)
        {
            return new Notification(data);
        }

        public byte[] Data { get; }

        /// <summary>
        /// set reply inside the received event to send a message back to tello
        /// </summary>
        public byte[] Reply { get; set; }
    }
}
