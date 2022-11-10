namespace Converter.ApiServer.Services.Interfaces
{
    /// <summary>
    /// File publishing service
    /// </summary>
    public interface IPublishFileService
    {
        /// <summary>
        /// Save file in storage service and notify user
        /// </summary>
        /// <param name="bytes">File in bytes</param>
        /// <param name="baseFileName">Filename without extension</param>
        /// <param name="fileHash">File identifier</param>
        /// <param name="userToken">User identifier</param>
        void PublishFileForUser(byte[] bytes, string baseFileName, int fileHash, string userToken);
    }
}