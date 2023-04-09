using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Data;
using Movie_Exercise.Models;

namespace Movie_Exercise.ViewComponents
{
    public class OldestMovies : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public OldestMovies(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            // The 5 oldest movies base on release year
                var OldeststMovies = _db.Movies.Select(m => m).OrderBy(m => m.ReleaseYear).ThenBy(m => m.Title).Take(5).ToList();
            return View("Index", OldeststMovies);
        }

    }
}
