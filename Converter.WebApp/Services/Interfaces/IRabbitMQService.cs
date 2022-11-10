namespace Converter.WebApp.Services.Interfaces
{
    /// <summary>
    /// Describes the connection of the broker to the server
    /// </summary>
    public interface IMessageQueueListenService
    {
        /// <summary>
        /// Connect to the server and declare queue
        /// </summary>
        void ConnectAndSubscribe();
    }
}
