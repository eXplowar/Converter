using Hangfire;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using System.Reflection;
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

        #region Register RabbitMQ settings
        var rabbitSettings = configuration
            .GetSection("RabbitMQ")
            .Get<RabbitMqSettings>();

        _ = rabbitSettings ?? throw new ApplicationException("Failed to read RabbitMQ settings block in configuration file");

        services.AddSingleton(rabbitSettings);
        #endregion

        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(config =>
        {
            var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            config.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
        });

        services.AddHangfire(config => config.UseInMemoryStorage());
        services.AddHangfireServer();

        #region Max request limit
        const int maxRequestLimit = 209715200; // 200 MB

        services.Configure<IISServerOptions>(options =>
        {
            options.MaxRequestBodySize = maxRequestLimit;
        });

        services.Configure<KestrelServerOptions>(options =>
        {
            options.Limits.MaxRequestBodySize = maxRequestLimit;
        });

        services.Configure<FormOptions>(x =>
        {
            x.ValueLengthLimit = maxRequestLimit;
            x.MultipartBodyLengthLimit = maxRequestLimit;
            x.MultipartHeadersLengthLimit = maxRequestLimit;
        });
        #endregion

        services.AddTransient<IApi, WeatherApi>();
        services.AddTransient<IApi, ConvertHtmlToPdfApi>();
        services.AddTransient<IApi, DownloadApi>();
        services.AddTransient<IHtmlToPdfConverterService, PuppeteerConvertService>();
        services.AddSingleton<IBrowserService, BrowserService>();
        services.AddSingleton<IPublishFileService, PublishFileInMQService>();
        services.AddSingleton<IStorageService, MemoryStorageService>();
        services.AddSingleton<IMessageQueueService, RabbitMqService>();
    }
}