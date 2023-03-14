using Microsoft.AspNetCore.Mvc;

namespace Movie_Exercise.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
