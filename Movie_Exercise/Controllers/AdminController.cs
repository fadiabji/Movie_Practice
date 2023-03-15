using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Models;

namespace Movie_Exercise.Controllers
{
    public class AdminController : Controller
    {
        public ViewResult Index() => View();
    }
}
