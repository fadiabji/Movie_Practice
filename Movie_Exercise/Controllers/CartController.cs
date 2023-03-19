using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Movie_Exercise.Data;
using Movie_Exercise.Models;
using Movie_Exercise.Models.ViewModels;
using Movie_Exercise.Services;
using Movie_Exercise.SessionHelpers;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;

namespace Movie_Exercise.Controllers
{
    public class CartController : Controller
    {

        public readonly IMovieService _movieService;
        public readonly ApplicationDbContext _db;
        const string SessionKeyCart = "ShoppingCart";

        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        public CartController( IMovieService movieService, ApplicationDbContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _movieService = movieService;
            _db = db;
        }
        public IActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public IActionResult AddToCart(int id)
        {
            Movie movie = _movieService.GetMovieById(id);
            if (HttpContext.Session.Get<List<Movie>>(SessionKeyCart) != null)
            {
                var oldSession = HttpContext.Session.Get<List<Movie>>(SessionKeyCart).ToList();
                oldSession.Add(movie);
                var newSession = oldSession;
                HttpContext.Session.Remove(SessionKeyCart);
                HttpContext.Session.Set<List<Movie>>(SessionKeyCart, newSession);
            }
            else
            {
                List<Movie> newMovieList = new List<Movie>();
                newMovieList.Add(movie);
                HttpContext.Session.Set<List<Movie>>(SessionKeyCart, newMovieList);
            }
            return Json(new { Value = HttpContext.Session.Get<List<Movie>>(SessionKeyCart).ToList() });
        }

        

        public IActionResult ViewCart()
        {
            //To Get Value from Session
            List<Movie> movieList = HttpContext.Session.Get<List<Movie>>(SessionKeyCart);
            if (movieList == null)
            {
                var empytEnumObj = Enumerable.Empty<CartItemVM>();
                return View(empytEnumObj);
            }
            else
            {
                var ItemsVMList = from m in movieList
                                  group m by m.Id into g
                                  orderby g.Key
                                  select new CartItemVM
                                  {
                                      Id = g.Key,
                                      MovieId = g.Select(m => m.Id).FirstOrDefault(),
                                      MovieTitle = g.Select(m => m.Title).FirstOrDefault(),
                                      Price = g.Select(m => m.Price).FirstOrDefault(),
                                      Quantity = g.Count(),
                                      ImgFile = g.Select(m => m.ImageFile).FirstOrDefault()
                                  };
                return View(ItemsVMList);
            }
        }
        public IActionResult IncrementCartItem(int Id)
        {
            var IncrementMovie = _movieService.GetMovieById(Id);
            List<Movie> movieList = HttpContext.Session.Get<List<Movie>>(SessionKeyCart);
            movieList.Add(IncrementMovie);
            HttpContext.Session.Remove(SessionKeyCart);
            HttpContext.Session.Set<List<Movie>>(SessionKeyCart, movieList);
            // return quantity for the item 
            // how much quantity for the movie have the id
            var ItemsVMList = from m in movieList
                              group m by m.Id into g
                              orderby g.Key
                              select new CartItemVM
                              {
                                  Id = g.Key,
                                  MovieId = g.Select(m => m.Id).FirstOrDefault(),
                                  MovieTitle = g.Select(m => m.Title).FirstOrDefault(),
                                  Price = g.Select(m => m.Price).FirstOrDefault(),
                                  Quantity = g.Count(),
                                  ImgFile = g.Select(m => m.ImageFile).FirstOrDefault()
                              };
            var quantity = ItemsVMList.Where(m => m.Id == Id).Select(m => m.Quantity).FirstOrDefault();

            return Json(new { Value = quantity });

            //return RedirectToAction("ViewCart");
        }

        public IActionResult DecrementCartItem(int Id)
        {
                List<Movie> movieList = HttpContext.Session.Get<List<Movie>>(SessionKeyCart);
                Movie newMovie = movieList.Find(m => m.Id == Id);
                movieList.Remove(newMovie);
                HttpContext.Session.Remove(SessionKeyCart);
                HttpContext.Session.Set<List<Movie>>(SessionKeyCart, movieList);
                //return RedirectToAction("ViewCart");
            var ItemsVMList = from m in movieList
                              group m by m.Id into g
                              orderby g.Key
                              select new CartItemVM
                              {
                                  Id = g.Key,
                                  MovieId = g.Select(m => m.Id).FirstOrDefault(),
                                  MovieTitle = g.Select(m => m.Title).FirstOrDefault(),
                                  Price = g.Select(m => m.Price).FirstOrDefault(),
                                  Quantity = g.Count(),
                                  ImgFile = g.Select(m => m.ImageFile).FirstOrDefault()
                              };
            var quantity = ItemsVMList.Where(m => m.Id == Id).Select(m => m.Quantity).FirstOrDefault();

            return Json(new { Value = quantity });

        }


        public IActionResult Checkout(CheckOutCustomerVM obj)
        {
            return View();
        }


        // Assuming you have a DbContext named MyDbContext and an entity named User with an Email property

        [HttpPost]
        public async Task<IActionResult> CheckEmailExists(string email)
        {
            // Query the UserManager for a user with the given email
            var user = await _userManager.FindByEmailAsync(email);
            if (user != null)
            {
                // Email already exists in the database
                return Json(true);
            }
            // Email does not exist in the database
            return Json(false);
        }


        //if a customer is alrady an user so save the customer details and don't ask for the details unless the customer doesn't ask for edit.
        //if a user but not customer 
        // if customer but not a user 
        

    }
}
