using RestSharp;
using System.Net;
using System.Net.Mime;
using System.Text;

namespace Converter.WebApp.Services
{
    /// <inheritdoc cref="IFileUploadService"/>
    public class FileUploadService : IFileUploadService
    {
        private readonly IApiService _apiService;

        public FileUploadService(IApiService apiService)
        {
            _apiService = apiService;
        }

        public async Task<ResponseDTO> UploadFile(IFormFile formFile, string userToken)
        {
            using var stream = formFile.OpenReadStream();
            using var streamReader = new StreamReader(stream);

            string fileContent = await streamReader.ReadToEndAsync();
            byte[] bytes = Encoding.UTF8.GetBytes(fileContent);

            var restRequest = new RestRequest("ConvertHtmlToPdf", Method.Post)
                .AddFile(formFile.FileName, bytes, formFile.FileName, formFile.ContentType)
                .AddHeader("token", userToken);

            var restResponse = await _apiService.RestClient.ExecuteAsync<ResponseDTO>(restRequest);

            if(restResponse.StatusCode != HttpStatusCode.OK || restResponse.Data is null)
            {
                restResponse.Data = new ResponseDTO { Status = ConversionStatus.Failed };
            }

            restResponse.Data.Token ??= userToken;

            return restResponse.Data;
        }

        public async Task<ResponseDTO> DownloadFile(int fileHash, string userToken)
        {
            var restRequest = new RestRequest("DownloadPdf", Method.Get)
                .AddQueryParameter("file", fileHash)
                .AddHeader("token", userToken);

            var response = await _apiService.RestClient.ExecuteAsync(restRequest);

            if (response.StatusCode != HttpStatusCode.OK)
                return new ResponseDTO { Status = ConversionStatus.Failed };
            
            string fileName = GetFileNameFromResponse(response);

            var result = new ResponseDTO
            {
                Status = ConversionStatus.Success,
                Token = userToken,
                Attachment = new AttachmentDTO { Blob = response.RawBytes, FileName = fileName }
            };

            return result;
        }

        /// <summary>
        /// Getting filename from RestResponse
        /// </summary>
        /// <param name="response">RestResponse</param>
        /// <returns></returns>
        private static string GetFileNameFromResponse(RestResponse response)
        {
            var headerValue = response.ContentHeaders.FirstOrDefault(x => x.Name == "Content-Disposition")?.Value;
            var contentDisposition = new ContentDisposition(Convert.ToString(headerValue));
            var fileName = contentDisposition.FileName;

            var correctFileName = contentDisposition.Parameters.ContainsKey("filename*")
                ? Uri.UnescapeDataString(contentDisposition.Parameters["filename*"])
                    .Replace("UTF-8''", string.Empty)
                    : contentDisposition.FileName;

            return correctFileName;
        }
    }
}
