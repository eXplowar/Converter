using PuppeteerSharp;

namespace Converter.ApiServer.Services
{
    /// <summary>
    /// Lazy implementation of browser service
    /// </summary>
    public class BrowserService : IBrowserService
    {
        private static IBrowser _browser = null;

        Lazy<Task<IBrowser>> lazyBrowser = new(async () => await InitializeAsync(), true);
        
        public async Task<IBrowser> GetBrowserAsync() => await lazyBrowser.Value;

        private static async Task<IBrowser> InitializeAsync()
        {
            if (_browser is not null)
            {
                return _browser;
            }

            var logger = GetLogger<BrowserService>();

            var browserFetcher = new BrowserFetcher();
            var revisionInfo = await browserFetcher.DownloadAsync();

            logger.LogInformation($"FolderPath: {revisionInfo.FolderPath}. \nExecutablePath: {revisionInfo.ExecutablePath}. \nUrl: {revisionInfo.Url}. \nPlatform: {revisionInfo.Platform}. \nDownloaded: {revisionInfo.Downloaded}.");

            var launchOptions = new LaunchOptions
            {
                Headless = true,
                ExecutablePath = revisionInfo.ExecutablePath,
                Args = new[] { "--no-sandbox" }
            };

            _browser = await Puppeteer.LaunchAsync(launchOptions);

            logger.LogInformation($"Browser launched. Version: {await _browser.GetVersionAsync()}. ExecutablePath: {revisionInfo.ExecutablePath}");

            return _browser;
        }

        private static ILogger<T> GetLogger<T>()
        {
            ILogger<T> logger;
            using (var serviceScope = ServiceLocator.GetScope())
            {
                logger = serviceScope.ServiceProvider.GetService<ILogger<T>>();
            }

            return logger;
        }
    }
}
