using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReactAPP.Pages
{
    public class ReactInterviewModel : PageModel
    {
        private readonly ILogger<ReactInterviewModel> _logger;

        public ReactInterviewModel(ILogger<ReactInterviewModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("React Interview Questions page loaded");
        }
    }
}
