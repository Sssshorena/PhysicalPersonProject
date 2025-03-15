using Microsoft.AspNetCore.Mvc;

namespace Project.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }
        public IActionResult Index()
        {
            
            return View();
        }

        public IActionResult OtherPage()
        {
            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
    }
}
