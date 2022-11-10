namespace Converter.ApiServer.Services.Interfaces
{
    /// <summary>
    /// Storing and returning files
    /// </summary>
    public interface IStorageService
    {
        /// <summary>
        /// Getting a file from storage
        /// </summary>
        /// <param name="userToken">User identifier</param>
        /// <param name="fileHash">File identifier</param>
        /// <returns>Blob with file in bytes</returns>
        Task<AttachmentDTO> GetFileAsync(string userToken, int fileHash);

        /// <summary>
        /// Save file in storage
        /// </summary>
        /// <param name="bytes">File in bytes</param>
        /// <param name="filename">Filename</param>
        /// <param name="fileHash">File identifier</param>
        /// <param name="userToken">User identifier</param>
        void SaveFile(byte[] bytes, string filename, int fileHash, string userToken);
    }
}