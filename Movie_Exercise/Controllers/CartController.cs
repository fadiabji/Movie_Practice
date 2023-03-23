using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient.Server;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Movie_Exercise.Data;
using Movie_Exercise.Models;
using Movie_Exercise.Models.ViewModels;
using Movie_Exercise.Services;
using Movie_Exercise.SessionHelpers;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography.X509Certificates;
using System.Text.Json;

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
        private readonly IMapper _mapper;
        
        public CartController(ICustomerService customerService,
                                        IMovieService movieService, 
                                        ApplicationDbContext db, 
                                        UserManager<AppUser> userManager, 
                                        SignInManager<AppUser> signInManager,
                                        IMapper mapper)
        {
            _customerService = customerService;
            _userManager = userManager;
            _signInManager = signInManager;
            _movieService = movieService;
            _db = db;
            _mapper = mapper;
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

            var customer = await Task.Run(() => _customerService.GetCustomerByEmail(email));
            if (customer != null)
            {
                // Email already exists in the database
                return Json(customer);
            }
            // Email does not exist in the database
            return Json(false);
        }


        // To payment method, take the customer obj from the form,
        // If customer is alrady exisit and didn't change anything so shift to payment 
        // If else customer is alreay exist but they change something in the details 
        // Make edit for the customer himself, then shift to payment
        // Else if cusomter donsn't exist, create a new customer then shift to payment method.

        [HttpPost]
        public async Task<IActionResult> CheckCustomer(Customer formData)
        {
            var mycusomer = await Task.Run(()=>_customerService.GetCustomerByEmail(formData.EmailAddress));
            if (mycusomer != null)
            {
                if(AreModelsIdentical<Customer>(mycusomer, formData))
                {
                    // redirect to payment method with formData
                    return RedirectToAction("MakePayment");
                }
                else
                {
                    // edit customer into new customer entry 
                    mycusomer.FirstName = formData.FirstName;
                    mycusomer.LastName = formData.LastName;
                    mycusomer.EmailAddress = formData.EmailAddress;
                    mycusomer.BillingAddress = formData.BillingAddress;
                    mycusomer.BillingCity = formData.BillingCity;
                    mycusomer.BillingZip = formData.BillingZip;
                    mycusomer.DeliveryAddress = formData.DeliveryAddress;
                    mycusomer.DeliveryCity = formData.DeliveryCity;
                    mycusomer.DeliveryZip = formData.DeliveryZip;
                    mycusomer.PhoneNumber = formData.PhoneNumber;
                    //convert the object into json in order to send to the payment method
                    string customerJson = JsonSerializer.Serialize<Customer>(mycusomer);

                    // redirect to paymnet method with formData
                    return RedirectToAction("MakePayment" , new { customerJson = customerJson });
                }
            }
            else
            {
                // create a new customer with formData
                Customer newCustomer = new() { 
                    FirstName = formData.FirstName,
                    LastName = formData.LastName,
                    BillingAddress = formData.BillingAddress,
                    BillingCity = formData.BillingCity,
                    BillingZip = formData.BillingZip,
                    DeliveryAddress = formData.DeliveryAddress,
                    DeliveryCity = formData.DeliveryCity,
                    DeliveryZip = formData.DeliveryZip,
                    EmailAddress = formData.EmailAddress,
                    PhoneNumber = formData.PhoneNumber
                };

                //convert the object into json in order to send to the payment method
                string customerJson = JsonSerializer.Serialize<Customer>(newCustomer);

                // redirect to paymnet method with formData
                return RedirectToAction("MakePayment", new { customerJson = customerJson });
                // will save customer when payment done

                // don't save the customer in this point until the payment done
                //_customerService.AddCustomer(newCustomer);

            }
            
        }


        public bool AreModelsIdentical<T>(T model1, T model2)
        {
            // get the properties of the model type using reflection
            var properties = typeof(T).GetProperties();

            // compare the properties of the two models
            foreach (var property in properties)
            {
                if (property.Name == "Id")
                {
                    // ignore the Id property
                    continue;
                }
                var value1 = property.GetValue(model1);
                var value2 = property.GetValue(model2);

                if (!object.Equals(value1, value2))
                {
                    // the two models are not identical
                    return false;
                }
            }

            // all properties are equal, the two models are identical
            return true;
        }


        [HttpGet]
        public IActionResult MakePayment(string customerJson)
        {
            return View(customerJson);
        }

        [HttpPost]
        public IActionResult MakePayment(string customerJson, bool IsPaymentDone)
        {
            if (customerJson != null)
            {
                Customer customer = JsonSerializer.Deserialize<Customer>(customerJson);
                if(_customerService.GetCustomerByEmail(customer.EmailAddress) != null)
                {
                    _customerService.UpdateCustomer(customer);
                    return View();
                }
                else
                {
                    _customerService.AddCustomer(customer);
                    return View();
                }
            }
            else
            {
                return View();
            }
        }


        public bool IsPaymentDone()
        {

            return true;
        }



    }
}
