var builder = WebApplication.CreateBuilder(args);

ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

ConfigureApp(app);
ConfigureAutorun(app);

app.Run();

static void ConfigureAutorun(WebApplication app) =>
    app.Lifetime.ApplicationStarted.Register(() =>
    {
        var rabbitMQService = app.Services.GetService<IMessageQueueListenService>();
        rabbitMQService.ConnectAndSubscribe();
    });
