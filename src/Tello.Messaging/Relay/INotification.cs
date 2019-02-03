namespace Tello.Messaging
{
    public interface INotification
    {
        byte[] Data { get; }

        /// <summary>
        /// set reply inside the received event to send a message back to tello
        /// </summary>
        byte[] Reply { get; set; }
    }
}
