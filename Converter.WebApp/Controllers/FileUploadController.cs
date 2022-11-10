using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Converter.WebApp.Controllers
{
    public class FileUploadController : Controller
    {
        private readonly IFileUploadService _fileUploadService;

        public FileUploadController(IFileUploadService fileUploadService)
        {
            _fileUploadService = fileUploadService;
        }

        [HttpPost]
        [Route("/Files/Upload")]
        [RequestFormLimits(MultipartBodyLengthLimit = 209715200)]
        public async Task<IActionResult> Upload()
        {
            var attachedFiles = HttpContext.Request.Form.Files;
            HttpContext.Request.Headers.TryGetValue("token", out var userToken);

            if (attachedFiles.Count == 0)
                return BadRequest("No file received");

            if (attachedFiles.Count > 1)
                return BadRequest("You can send only one file");

            var formFile = attachedFiles.FirstOrDefault();

            if (StringValues.IsNullOrEmpty(userToken))
            {
                userToken = Guid.NewGuid().ToString();
            }

            var result = await _fileUploadService.UploadFile(formFile, userToken);

            return Ok(new { result.Status, result.Token });
        }

        [HttpGet]
        [Route("/Files/Download")]
        public async Task<IActionResult> Download([FromQuery(Name = "file")] int fileHash, [FromHeader] string token)
        {
            var result = await _fileUploadService.DownloadFile(fileHash, token);

            return result.Status == ConversionStatus.Failed || result.Attachment?.Blob == null
                ? BadRequest("Error when trying to download a file")
                : File(result.Attachment.Blob, "application/force-download", result.Attachment.FileName);
        }
    }
}
