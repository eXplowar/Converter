using Converter.WebApp.Hubs;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

public partial class Program
{
    /// <summary>
    /// Registration and configuration of services
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="configuration">Configuration of application</param>
    /// <exception cref="ApplicationException">Thow if sone configuration not found in appsettings.json</exception>
    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<JsonOptions>(config =>
        {
            config.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
        });

        #region Register ApiServer settings
        var apiServerSettings = configuration
            .GetSection("ApiServer")
            .Get<ApiServerSettings>();

        _ = apiServerSettings ?? throw new ApplicationException("Failed to read ApiServer settings block in configuration file");

        services.AddSingleton(apiServerSettings);
        #endregion

        #region Register RabbitMQ settings
        var rabbitSettings = configuration
            .GetSection("RabbitMQ")
            .Get<RabbitMqSettings>();

        _ = rabbitSettings ?? throw new ApplicationException("Failed to read RabbitMQ settings block in configuration file");

        services.AddSingleton(rabbitSettings);
        #endregion

        services.AddRazorPages();
        services.AddSignalR();
        services.AddHealthChecks();

        services.AddTransient<IFileUploadService, FileUploadService>();
        services.AddSingleton<IApiService, RestClientService>();
        services.AddSingleton<IMessageQueueListenService, RabbitMQService>();
        services.AddSingleton<RepositoryHub>();
    }
}
