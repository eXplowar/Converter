using Hangfire;

namespace Converter.ApiServer.Services
{
    /// <summary>
    /// Puppeteer implementation of IHtmlToPdfConverterService for conversion HTML to PDF
    /// </summary>
    public class PuppeteerConvertService : IHtmlToPdfConverterService
    {
        private readonly IBrowserService _browserService;
        private readonly IPublishFileService _returnFileService;

        public PuppeteerConvertService(IBrowserService browserService, IPublishFileService returnFileService)
        {
            _browserService = browserService;
            _returnFileService = returnFileService;
        }

        public async Task<ResponseDTO> ConvertHtmlToPdfQueuedAsync(string fileContent, string fileName, int fileHash, string userToken)
        {
            var jobId = BackgroundJob.Enqueue(() => MakeAndPublishPdfAsync(fileContent, fileName, fileHash, userToken));

            var result = new ResponseDTO
            {
                Status = ConversionStatus.InProgress,
                Token = userToken
            };

            return await Task.FromResult(result);
        }

        public async Task MakeAndPublishPdfAsync(string fileContent, string fileName, int fileHash, string userToken)
        {
            var browser = await _browserService.GetBrowserAsync();
            await using var page = await browser.NewPageAsync();
            await page.SetContentAsync(fileContent);
            var pdfStream = await page.PdfStreamAsync();

            int fileExtPos = fileName.LastIndexOf(".");

            var baseFileName = fileExtPos >= 0
                ? fileName[..fileExtPos]
                : fileName;

            using var memoryStream = new MemoryStream();
            pdfStream.CopyTo(memoryStream);
            byte[] bytes = memoryStream.ToArray();
            memoryStream.Close();

            _returnFileService.PublishFileForUser(bytes, baseFileName, fileHash, userToken);
        }
    }
}