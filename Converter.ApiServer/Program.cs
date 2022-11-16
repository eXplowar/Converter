var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureApp(app);

RegisterApi(app);

app.Run();

static void RegisterApi(WebApplication app)
{
    var apis = app.Services.GetServices<IApi>();

    if (!apis.Any()) throw new ApplicationException("Api not found");

    foreach (var api in apis)
    {
        api.Register(app);
    }
}
