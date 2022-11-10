var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureApp(app);

app.Run();

static void ConfigureApp(WebApplication app)
{
    app.MapReverseProxy();
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services
        .AddReverseProxy()
        .LoadFromConfig(configuration.GetSection("ReverseProxy"));
}