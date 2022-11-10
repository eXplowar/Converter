using Converter.WebApp.Hubs;
using Microsoft.AspNetCore.Http.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureApp(app);
ConfigureAutorun(app);

app.Run();

static void ConfigureApp(WebApplication app)
{
    // Configure the HTTP request pipeline.
    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Error");
    }

    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthorization();

    app.MapRazorPages();
    app.MapHub<RepositoryHub>("/repositoryHub");

    app.UseHealthChecks("/health");

    app.MapDefaultControllerRoute();
}

static void ConfigureAutorun(WebApplication app) =>
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var rabbitMQService = app.Services.GetService<IMessageQueueListenService>();
        rabbitMQService.ConnectAndSubscribe();
    });


static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
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
