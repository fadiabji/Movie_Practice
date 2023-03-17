using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Models;

namespace Movie_Exercise.Controllers
{
    [Authorize(Roles = ("Admin"))]
    public class AdminController : Controller
    {
        public ViewResult Index() => View();
    }
}
