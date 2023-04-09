using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Data;
using Movie_Exercise.Models;

namespace Movie_Exercise.ViewComponents
{
    public class CheapestMovies : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public CheapestMovies(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            //    // The 5 Cheapest movies based on price
                var cheapestMovies = _db.Movies.Select(m => m).OrderBy(m => m.Price).ThenBy(m => m.Title).Take(5).ToList();
            return View("Index", cheapestMovies);
        }

    }
}
