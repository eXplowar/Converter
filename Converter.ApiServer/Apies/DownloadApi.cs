using Microsoft.AspNetCore.Mvc;

namespace Converter.ApiServer.Apies
{
    /// <summary>
    /// API for downloading files
    /// </summary>
    public class DownloadApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/DownloadPdf", DownloadPdf)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Download PDF file
        /// </summary>
        /// <param name="fileHash">File identifier</param>
        /// <param name="token">User identifier</param>
        /// <param name="storageService">Storage service from IoC-container</param>
        /// <returns>PDF file</returns>
        private async Task<IResult> DownloadPdf([FromQuery(Name = "file")] int fileHash, [FromHeader] string token,
            [FromServices] IStorageService storageService)
        {
            if (string.IsNullOrEmpty(token)) return Results.BadRequest("No user token received");

            var result = await storageService.GetFileAsync(token, fileHash);

            if (result is null) return Results.BadRequest("No file found for this user");

            return Results.File(result.Blob, "application/force-download", result.FileName);
        }
    }
}
