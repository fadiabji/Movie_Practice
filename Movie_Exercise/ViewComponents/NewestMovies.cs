using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Data;
using Movie_Exercise.Models;

namespace Movie_Exercise.ViewComponents
{
    public class NewestMovies : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public NewestMovies(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            // The 5 newest movies based on release year
            var newestMovies = _db.Movies.Select(m => m).OrderByDescending(m => m.ReleaseYear).ThenBy(m => m.Title).Take(5).ToList();
            return View("Index", newestMovies);
        }

    }
}
