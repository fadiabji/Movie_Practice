using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Models;
using System.Diagnostics;

namespace Movie_Exercise.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }


        //public IActionResult Index()
        //{
        //    int mostPopularMovieId = (from or in _db.OrderRows
        //                              group or by or.MovieId into g
        //                              orderby g.Count() descending
        //                              select new
        //                              {
        //                                  movieId = g.Select(or => or.MovieId).FirstOrDefault()
        //                              }).ToList().Select(c => c.movieId).FirstOrDefault();

        //    Movie MostPopularMovie = _db.Movies.Where(m => m.Id == mostPopularMovieId).FirstOrDefault();
        //    // The 5 newest movies based on release year
        //    var newestMovies = _db.Movies.Select(m => m).OrderByDescending(m => m.ReleaseYear).ThenBy(m => m.Title).Take(5).ToList();
        //    // The 5 oldest movies base on release year
        //    var OldeststMovies = _db.Movies.Select(m => m).OrderBy(m => m.ReleaseYear).ThenBy(m => m.Title).Take(5).ToList();
        //    // The 5 Cheapest movies based on price
        //    var CheapestMovies = _db.Movies.Select(m => m).OrderBy(m => m.Price).ThenBy(m => m.Title).Take(5).ToList();
        //    // The Customer who has made the most expensive order (based on the sum of the order rows in an order)
        //    var listofAllOrders = _db.Orders.Select(o => o)
        //                                   .Include(o => o.OrderRows)
        //                                   .ToList();

        //    decimal maxOrderTotal = 0;
        //    Order maxExpensiveOrder = new Order();
        //    foreach (var order in listofAllOrders)
        //    {
        //        // here I will get the order which has a biggest total price
        //        var minOrderTotal = order.OrderRows.Sum(or => or.Price);
        //        if (minOrderTotal > maxOrderTotal)
        //        {
        //            maxOrderTotal = minOrderTotal;
        //            maxExpensiveOrder = order;
        //        }
        //    }
        //    Customer topCustomer = _db.Customers.Where(c => c.Id == maxExpensiveOrder.CustomerId).FirstOrDefault();
        //    TopCustomerVM cutomer = new TopCustomerVM();
        //    cutomer.order = maxExpensiveOrder;
        //    cutomer.customer = topCustomer;
        //    var homeViewTuple = new Tuple<Movie, List<Movie>, List<Movie>, List<Movie>, TopCustomerVM>(MostPopularMovie, newestMovies, OldeststMovies, CheapestMovies, cutomer);
        //    return View(homeViewTuple);
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}