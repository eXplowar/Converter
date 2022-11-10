namespace Converter.ApiServer.Services.Interfaces
{
    /// <summary>
    /// Message queue service
    /// </summary>
    public interface IMessageQueueService
    {
        /// <summary>
        /// Send complex object
        /// </summary>
        /// <param name="obj">Complex object. Can use for serialization and send to MQ</param>
        void SendMessage(object obj);

        /// <summary>
        /// Send text message
        /// </summary>
        /// <param name="message">Text message</param>
        void SendMessage(string message);
    }
}
