using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReactAPP.Pages
{
    public class ReactAdvancedModel : PageModel
    {
        private readonly ILogger<ReactAdvancedModel> _logger;

        public ReactAdvancedModel(ILogger<ReactAdvancedModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("React Advanced page loaded");
        }
    }
}
