using PuppeteerSharp;

namespace Converter.ApiServer.Services.Interfaces
{
    /// <summary>
    /// The service provides a browser
    /// </summary>
    public interface IBrowserService
    {
        /// <summary>
        /// Return browser lazy
        /// </summary>
        /// <returns>Browser</returns>
        Task<IBrowser> GetBrowserAsync();
    }
}