using Microsoft.AspNetCore.Mvc;

namespace Movie_Exercise.Controllers
{
    public class CustomersController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Crrate()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create()
        {
            return View();
        }
    }
}
