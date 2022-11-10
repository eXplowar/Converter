namespace Converter.ApiServer.Services.Interfaces
{
    /// <summary>
    /// HTML to PDF converter service
    /// </summary>
    public interface IHtmlToPdfConverterService
    {
        /// <summary>
        /// Convert HTML to PDF using <b>queue engine</b>
        /// </summary>
        /// <param name="fileContent">HTMl-content</param>
        /// <param name="fileName">Filename</param>
        /// <param name="fileHash">File identifier</param>
        /// <param name="userToken">User identifier</param>
        /// <returns>No return file</returns>
        Task<ResponseDTO> ConvertHtmlToPdfQueuedAsync(string fileContent, string fileName, int fileHash, string userToken);

        /// <summary>
        /// Creating and publishing PDF
        /// </summary>
        /// <param name="fileContent">HTMl-content</param>
        /// <param name="fileName">Filename</param>
        /// <param name="fileHash">File identifier</param>
        /// <param name="userToken">User identifier</param>
        /// <returns>No return file</returns>
        Task MakeAndPublishPdfAsync(string fileContent, string fileName, int fileHash, string userToken);
    }
}