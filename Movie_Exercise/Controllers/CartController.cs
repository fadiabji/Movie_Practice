using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
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
        private readonly ICustomerService _customerService;
        public CartController(ICustomerService customerService, IMovieService movieService, ApplicationDbContext db, UserManager<AppUser> userManager, SignInManager<AppUser> signInManager)
        {
            _customerService = customerService;
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

            var customer = await Task.Run(()=> _customerService.GetCustomerByEmail(email));
            if (customer != null)
            {
                // Email already exists in the database
                return Json(customer);
            }
            // Email does not exist in the database
            return Json(false);
        }


        // To payment method, take the customer obj from the form,
        // if customer is alrady exisit and didn't change anything so shift to payment 
        // if else customer is alreay exist but they change something in the details 
        // make edit for the customer himself, then shift to payment
        // else if cusomter donsn't exist, create a new customer then shift to payment method.

        [HttpPost]
        public async void CheckCustomer(Customer formData)
        {
            //var customerForm = new Customer(formData.EmailAddress, formData.FirstName, formData.LastName, formData.PhoneNumber,
            //                                    formData.BillingAddress, formData.BillingCity, formData.BillingZip, 
            //                                    formData.DeliveryAddress, formData.DeliveryCity, formData.DeliveryZip);

            var mycusomer = await Task.Run(()=>_customerService.GetCustomerByEmail(formData.EmailAddress));
            if (mycusomer != null)
            {
                if(ObjectsEqual(formData, mycusomer))
                {
                    // redirect to payment method with formData
                }
                else
                {
                    // redirect to edit customer methon in customer controller
                    // make edit customer acording to last form update
                    // save changes in customers table
                    // redirect to paymnet method with formData
                }
            }
            else
            {
                // redirect to create a new customer with formData
                // don't save the customer in this point 
                // will save customer when payment done

            }
            
        }


        public bool ObjectsEqual(object obj1, object obj2)
        {
            // Get the property names of obj1
            var obj1Props = obj1.GetType().GetProperties();

            // Check that obj2 has the same number of properties as obj1
            if (obj1Props.Length != obj2.GetType().GetProperties().Length)
            {
                return false;
            }

            // Check that each property in obj1 has the same value as the corresponding property in obj2
            foreach (var prop in obj1Props)
            {
                if (prop.GetValue(obj1, null) != prop.GetValue(obj2, null))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
