using Hangfire;

public partial class Program
{
    /// <summary>
    /// Application configuration
    /// </summary>
    /// <param name="app">The web application used to configure the HTTP pipeline, and routes</param>
    private static void ConfigureApp(WebApplication app)
    {
        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHangfireDashboard();

        ServiceLocator.Configure(app.Services);
    }
}