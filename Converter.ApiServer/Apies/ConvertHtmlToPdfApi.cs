using Microsoft.AspNetCore.Mvc;

namespace Converter.ApiServer.Apies
{
    /// <summary>
    /// API for converting HTML to PDF
    /// </summary>
    public class ConvertHtmlToPdfApi : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapPost("/ConvertHtmlToPdf", ConvertHtmlToPdf)
                .Produces(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status400BadRequest);
        }

        /// <summary>
        /// Convert HTML-file to PDF
        /// </summary>
        /// <param name="request">Internal HttpRequest</param>
        /// <param name="htmlToPdfConverterService">Service provide converting</param>
        /// <returns></returns>
        private async Task<IResult> ConvertHtmlToPdf(HttpRequest request,
            [FromServices] IHtmlToPdfConverterService htmlToPdfConverterService)
        {
            var attachedFiles = request.Form.Files;
            request.Headers.TryGetValue("token", out var userToken);

            if (string.IsNullOrEmpty(userToken))
                return Results.BadRequest("No user token received");

            if (attachedFiles.Count == 0)
                return Results.BadRequest("No file received");

            if (attachedFiles.Count > 1)
                return Results.BadRequest("You can send only one file");

            var formFile = attachedFiles.FirstOrDefault();

            var fileHash = formFile.GetHashCode();

            var streamReader = new StreamReader(formFile.OpenReadStream());
            string fileContent = await streamReader.ReadToEndAsync();

            var result = await htmlToPdfConverterService.ConvertHtmlToPdfQueuedAsync(fileContent, formFile.FileName, fileHash, userToken);

            return Results.Ok(result);
        }
    }
}
