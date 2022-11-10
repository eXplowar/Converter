namespace Converter.WebApp.Services.Interfaces
{
    /// <summary>
    /// Upload service
    /// </summary>
    public interface IFileUploadService
    {
        /// <summary>
        /// Upload file
        /// </summary>
        /// <param name="formFile">Attached file</param>
        /// <returns>Upload result</returns>
        Task<ResponseDTO> UploadFile(IFormFile formFile, string userToken);

        /// <summary>
        /// Download file
        /// </summary>
        /// <param name="fileHash">File identifier</param>
        /// <param name="userToken">User identifier</param>
        /// <returns>DTO with attachment</returns>
        Task<ResponseDTO> DownloadFile(int fileHash, string userToken);
    }
}