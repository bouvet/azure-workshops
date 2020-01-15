using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;

namespace AzureWorkshopApp.Controllers
{
    [Authorize] // Må være innlogget for
    public class HomeController : Controller
    {
        private readonly IConfiguration _configuration;

        public HomeController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public IActionResult Index()
        {
            ViewData.Add("test", _configuration["Parent:ChildOne"]);

            return View();
        }
    }
}