namespace Tello.Udp
{
    public class DataNotification
    {
        private DataNotification(byte[] data) { Data = data; }

        public static DataNotification FromData(byte[] data)
        {
            return new DataNotification(data);
        }

        public byte[] Data { get; }

        /// <summary>
        /// set reply inside the received event to send a message back to tello
        /// </summary>
        public byte[] Reply { get; set; }
    }
}