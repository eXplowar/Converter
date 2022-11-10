namespace Converter.ApiServer.Services
{
    /// <summary>
    /// MQ implementation of IPublishFileService for notification about the completion of the conversion
    /// </summary>
    public class PublishFileInMQService : IPublishFileService
    {
        private readonly IMessageQueueService _mqService;
        private readonly IStorageService _storageService;

        public PublishFileInMQService(IMessageQueueService mqService, IStorageService storageService)
        {
            _mqService = mqService;
            _storageService = storageService;
        }

        public void PublishFileForUser(byte[] bytes, string baseFileName, int fileHash, string userToken)
        {
            var filename = $"{baseFileName}.pdf";
            _storageService.SaveFile(bytes, filename, fileHash, userToken);

            _mqService.SendMessage(new { FileName = filename, FileHash = fileHash, Token = userToken });
        }
    }
}
