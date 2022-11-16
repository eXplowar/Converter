using Converter.WebApp.Hubs;

public partial class Program
{
    /// <summary>
    /// Application configuration
    /// </summary>
    /// <param name="app">The web application used to configure the HTTP pipeline, and routes</param>
    private static void ConfigureApp(WebApplication app)
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
}
