using Microsoft.AspNetCore.Mvc.RazorPages;

namespace ReactAPP.Pages
{
    public class ReactLearningModel : PageModel
    {
        private readonly ILogger<ReactLearningModel> _logger;

        public ReactLearningModel(ILogger<ReactLearningModel> logger)
        {
            _logger = logger;
        }

        public void OnGet()
        {
            _logger.LogInformation("React Learning page loaded");
        }
    }
}
