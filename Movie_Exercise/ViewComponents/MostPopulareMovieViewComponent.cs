using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Data;
using Movie_Exercise.Models;

namespace Movie_Exercise.ViewComponents
{
    public class MostPopulareMovieViewComponent : ViewComponent
    {

        private readonly ApplicationDbContext _db;
        public MostPopulareMovieViewComponent(ApplicationDbContext db)
        {
            _db = db;
        }

        public IViewComponentResult Invoke()
        {
            int mostPopularMovieId = (from or in _db.OrderRows
                                      group or by or.MovieId into g
                                      orderby g.Count() descending
                                      select new
                                      {
                                          movieId = g.Select(or => or.MovieId).FirstOrDefault()
                                      }).ToList().Select(c => c.movieId).FirstOrDefault();

            Movie mostPopularMovie = _db.Movies.Where(m => m.Id == mostPopularMovieId).FirstOrDefault();
            return View("Index", mostPopularMovie);
        }
    }
}



