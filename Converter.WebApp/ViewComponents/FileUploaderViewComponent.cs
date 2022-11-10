using Microsoft.AspNetCore.Mvc;

namespace Converter.WebApp.ViewComponents
{
    public class FileUploaderViewComponent : ViewComponent
    {
        private readonly ILogger<FileUploaderViewComponent> _logger;

        public FileUploaderViewComponent(ILogger<FileUploaderViewComponent> logger)
        {
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync() => View();
    }
}
