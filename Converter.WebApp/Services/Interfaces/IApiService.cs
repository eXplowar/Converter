using RestSharp;

namespace Converter.WebApp.Services.Interfaces
{
    /// <summary>
    /// Service for calls to the API server
    /// </summary>
    public interface IApiService
    {
        RestClient RestClient { get; }
    }
}