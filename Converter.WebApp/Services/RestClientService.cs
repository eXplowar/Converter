using Microsoft.AspNetCore.Http.Json;
using Microsoft.Extensions.Options;
using RestSharp;
using RestSharp.Serializers.Json;

namespace Converter.WebApp.Services
{
    /// <summary>
    /// Service provide access to RestClient
    /// </summary>
    public class RestClientService : IApiService
    {
        private readonly RestClient _restClient;

        public RestClientService(ApiServerSettings apiServerSettings, IOptions<JsonOptions> options)
        {
            _restClient = new RestClient(apiServerSettings.Url);
            _restClient.UseSystemTextJson(options.Value.SerializerOptions);
        }

        /// <summary>
        /// RestClient
        /// </summary>
        public RestClient RestClient => _restClient;
    }
}
