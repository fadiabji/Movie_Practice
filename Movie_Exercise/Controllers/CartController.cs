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
        private readonly IOrderService _orderService;
        
        public CartController(ICustomerService customerService,
                                        IMovieService movieService, 
                                        ApplicationDbContext db, 
                                        UserManager<AppUser> userManager, 
                                        SignInManager<AppUser> signInManager,
                                        IMapper mapper,
                                        IOrderService orderService)
        {
            _customerService = customerService;
            _userManager = userManager;
            _signInManager = signInManager;
            _movieService = movieService;
            _db = db;
            _mapper = mapper;
            _orderService = orderService;
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
                    string customerJson = JsonSerializer.Serialize<Customer>(mycusomer);
                    // redirect to payment method with formData
                    return RedirectToAction("MakePayment", new { customerJson = customerJson });

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
            CustomerJson customer = new (){ CustomerStringObj = customerJson };
            //ViewBag.Customer = customerJson;
            return View(customer);
        }

        [HttpPost]
        public IActionResult MakePaymentBtn(string customerJson)
        {
            if (customerJson != null)
            {
                Customer customer = JsonSerializer.Deserialize<Customer>(customerJson);
                if (_customerService.GetCustomerByEmail(customer.EmailAddress) != null)
                {
                    _customerService.UpdateCustomer(customer);
                    PlaceOrder(customer.Id);
                    return View();
                }
                else
                {
                    _customerService.AddCustomer(customer);
                    PlaceOrder(customer.Id);
                    return View();
                }
            }
            else
            {
                return RedirectToAction("ViewCart");
            }
        }

        //plance and order and save to database
        public void PlaceOrder(int customerId)
        {
            List<Movie> movieList = HttpContext.Session.Get<List<Movie>>(SessionKeyCart);
            Order order = new Order();
            order.CustomerId = customerId;
            order.OrderRows = movieList.Select(m => new OrderRow
            {
                MovieId = m.Id,
                Price = m.Price,
            }).ToList();
            _orderService.AddOrder(order);
            HttpContext.Session.Remove(SessionKeyCart);
        }


        public bool IsPaymentDone()
        {

            return true;
        }





        // payment through swish


        // MainTestPaymentAndRefund
        static void MainTestPayment()
        {
            var clientCertificate = new SwishApi.Models.ClientCertificate()
            {
                CertificateAsStream = System.IO.File.OpenRead("TestCert//Swish_Merchant_TestCertificate_1234679304.p12"),
                Password = "swish"
            };

            var eCommerceClient = new SwishApi.ECommerceClient(clientCertificate, "https://eofvqci6optquip.m.pipedream.net", "12345", "1234679304", true, SwishApi.Environment.Emulator);

            string instructionUUID = Guid.NewGuid().ToString("N").ToUpper();

            // Make the Payement Request
            var response = eCommerceClient.MakePaymentRequest("1234679304", 1, "Test", instructionUUID);

            // Check if the payment request got success and not got any error
            if (string.IsNullOrEmpty(response.Error))
            {
                // All OK
                string urlForCheckingPaymentStatus = response.Location;

                // If you do a webbapplication you here should wait some time, showing a "loading" view or something and try to do the payment
                // status check as below, you maybe have some ajax request doing a call to a actionresult doing this code
                // Wait so that the payment request has been processed
                System.Threading.Thread.Sleep(5000);

                // Make the payment status check
                var statusResponse = eCommerceClient.CheckPaymentStatus(urlForCheckingPaymentStatus);

                // Check if the call is done correct
                if (string.IsNullOrEmpty(statusResponse.errorCode))
                {
                    // Call was maked without any problem
                    Console.WriteLine("Status: " + statusResponse.status);

                    if (statusResponse.status == "PAID")
                    {
                        var refundClient = new SwishApi.RefundClient(clientCertificate, "https://eofvqci6optquip.m.pipedream.net", "1234", "1234679304", true, SwishApi.Environment.Emulator);

                        string instructionUUID2 = Guid.NewGuid().ToString("N").ToUpper();

                        var refundResponse = refundClient.MakeRefundRequest(statusResponse.paymentReference, "0731596605", (int)statusResponse.amount, "Återköp", instructionUUID2);

                        if (string.IsNullOrEmpty(refundResponse.Error))
                        {
                            // Request OK
                            string urlForCheckingRefundStatus = refundResponse.Location;

                            // If you do a webbapplication you here should wait some time, showing a "loading" view or something and try to do the refund status check as below, you maybe have some ajax request doing a call to a actionresult doing this code
                            // Wait so that the refund has been processed
                            System.Threading.Thread.Sleep(5000);

                            // Check refund status
                            var refundCheckResposne = refundClient.CheckRefundStatus(urlForCheckingRefundStatus);

                            if (string.IsNullOrEmpty(refundCheckResposne.errorCode))
                            {
                                // Call was maked without any problem
                                Console.WriteLine("RefundChecKResponse - Status: " + statusResponse.status);
                            }
                            else
                            {
                                // ERROR
                                Console.WriteLine("RefundCheckResponse: " + refundCheckResposne.errorCode + " - " + refundCheckResposne.errorMessage);
                            }
                        }
                        else
                        {
                            // ERROR
                            Console.WriteLine("Refund Error: " + refundResponse.Error);
                        }
                    }
                }
                else
                {
                    // ERROR
                    Console.WriteLine("CheckPaymentResponse: " + statusResponse.errorCode + " - " + statusResponse.errorMessage);
                }
            }
            else
            {
                // ERROR
                Console.WriteLine("MakePaymentRequest - ERROR: " + response.Error);
            }


            Console.WriteLine(">>> Press enter to exit <<<");
            Console.ReadLine();
        }

        // MainTestQCommerce
        static void MainTestQCommerce()
        {
            var clientCertificate = new SwishApi.Models.ClientCertificate()
            {
                CertificateFilePath = "TestCert//Swish_Merchant_TestCertificate_1234679304.p12",
                Password = "swish"
            };

            var mCommerceClient = new SwishApi.MCommerceClient(clientCertificate, "https://eofvqci6optquip.m.pipedream.net", "12345", "1234679304", true, SwishApi.Environment.Emulator);

            string instructionUUID = Guid.NewGuid().ToString("N").ToUpper();

            var responseMCommerce = mCommerceClient.MakePaymentRequest(1, "Test", instructionUUID);

            var getQRCodeResponse = mCommerceClient.GetQRCode(responseMCommerce.Token, "svg");

            if (string.IsNullOrEmpty(getQRCodeResponse.Error))
            {
                System.IO.File.WriteAllText("test.svg", getQRCodeResponse.SVGData);

                // If you do a webbapplication you here should wait some time, showing a "loading" view or something and try to do the payment status check as below, you maybe have some ajax request doing a call to a actionresult doing this code
                // Wait so that the payment request has been processed
                System.Threading.Thread.Sleep(5000);

                // Make the payment status check
                var statusResponse = mCommerceClient.CheckPaymentStatus(responseMCommerce.Location);

                // Check if the call is done correct
                if (string.IsNullOrEmpty(statusResponse.errorCode))
                {
                    // Call was maked without any problem
                    Console.WriteLine("Status: " + statusResponse.status);

                    if (statusResponse.status == "PAID")
                    {
                        var refundClient = new SwishApi.RefundClient(clientCertificate, "https://eofvqci6optquip.m.pipedream.net", "1234", "1234679304", true, SwishApi.Environment.Emulator);

                        string instructionUUID2 = Guid.NewGuid().ToString("N").ToUpper();

                        var refundResponse = refundClient.MakeRefundRequest(statusResponse.paymentReference, "0731596605", (int)statusResponse.amount, "Återköp", instructionUUID2);

                        if (string.IsNullOrEmpty(refundResponse.Error))
                        {
                            // Request OK
                            string urlForCheckingRefundStatus = refundResponse.Location;

                            // If you do a webbapplication you here should wait some time, showing a "loading" view or something and try to do the refund status check as below, you maybe have some ajax request doing a call to a actionresult doing this code
                            // Wait so that the refund has been processed
                            System.Threading.Thread.Sleep(5000);

                            // Check refund status
                            var refundCheckResposne = refundClient.CheckRefundStatus(urlForCheckingRefundStatus);

                            if (string.IsNullOrEmpty(refundCheckResposne.errorCode))
                            {
                                // Call was maked without any problem
                                Console.WriteLine("RefundChecKResponse - Status: " + statusResponse.status);
                            }
                            else
                            {
                                // ERROR
                                Console.WriteLine("RefundCheckResponse: " + refundCheckResposne.errorCode + " - " + refundCheckResposne.errorMessage);
                            }
                        }
                        else
                        {
                            // ERROR
                            Console.WriteLine("Refund Error: " + refundResponse.Error);
                        }
                    }
                }
                else
                {
                    // ERROR
                    Console.WriteLine("CheckPaymentResponse: " + statusResponse.errorCode + " - " + statusResponse.errorMessage);
                }
            }
            else
            {
                Console.WriteLine("ERROR Get QR Code: " + getQRCodeResponse.Error);
            }

            Console.WriteLine(">>> Press enter to exit <<<");
            Console.ReadLine();
        }

        // Test cancel payment
        static void MainTestCancelPayment()
        {
            var clientCertificate = new SwishApi.Models.ClientCertificate()
            {
                CertificateAsStream = System.IO.File.OpenRead("TestCert//Swish_Merchant_TestCertificate_1234679304.p12"),
                Password = "swish"
            };

            var eCommerceClient = new SwishApi.ECommerceClient(clientCertificate, "https://eow7hpmlfs99yn0.m.pipedream.net", "12345", "1234679304", true, SwishApi.Environment.Emulator);

            string instructionUUID = Guid.NewGuid().ToString("N").ToUpper();

            // Make the Payement Request
            var response = eCommerceClient.MakePaymentRequest("1234679304", 1, "Test", instructionUUID);

            // Check if the payment request got success and not got any error
            if (string.IsNullOrEmpty(response.Error))
            {
                // All OK
                string paymentReferenceURL = response.Location;

                // {"id":"63C26F360CEA403B955FCDCCE7352502","payeePaymentReference":"12345","paymentReference":"59DD805C66F44665AD44A21460B3002A","callbackUrl":"https://eow7hpmlfs99yn0.m.pipedream.net","payerAlias":"1234679304","payeeAlias":"1234679304","amount":1.00,"currency":"SEK","message":"Test","status":"CANCELLED","dateCreated":"2022-10-20T08:38:36.749Z","datePaid":null,"errorCode":"RP08","errorMessage":"The payment request has been cancelled."}
                var cancelResponse = eCommerceClient.CancelPaymentRequest(paymentReferenceURL);

                if (!string.IsNullOrEmpty(cancelResponse.status) && cancelResponse.status == "CANCELLED")
                {
                    Console.WriteLine("Payment is cancelled!");
                }
                else
                {
                    Console.WriteLine("Something got wrong when try to cancel the paymnent: " + cancelResponse.ErrorMessage);
                }
            }
            else
            {
                // ERROR
                Console.WriteLine("MakePaymentRequest - ERROR: " + response.Error);
            }


            Console.WriteLine(">>> Press enter to exit <<<");
            Console.ReadLine();
        }

        static void MainTestPayout()
        {
            var clientCertificate = new SwishApi.Models.ClientCertificate()
            {
                CertificateFilePath = "TestCert//Swish_Merchant_TestCertificate_1234679304.p12",
                Password = "swish"
            };

            string certificatePath = Environment.CurrentDirectory + "\\TestCert\\Swish_Merchant_TestSigningCertificate_1234679304.p12";

            var payoutClient = new SwishApi.PayoutClient(clientCertificate, "https://eofvqci6optquip.m.pipedream.net", "1234", "1234679304", true, SwishApi.Environment.Emulator);

            string instructionUUID = Guid.NewGuid().ToString("N").ToUpper();

            // Test payeeAlias and payeeSSN from MSS_UserGuide_v1.9.pdf
            var response = payoutClient.MakePayoutRequest("46722334455", "197501088327", 1, "Test", instructionUUID, "7d70445ec8ef4d1e3a713427e973d097", new SwishApi.Models.ClientCertificate() { CertificateFilePath = certificatePath, Password = "swish" });

            if (string.IsNullOrEmpty(response.Error))
            {
                Console.WriteLine("Location: " + response.Location);

                // If you do a webbapplication you here should wait some time, showing a "loading" view or something and try to do the payment status check as below, you maybe have some ajax request doing a call to a actionresult doing this code
                // Wait so that the payment request has been processed
                System.Threading.Thread.Sleep(5000);

                // Make the payment status check
                var statusResponse = payoutClient.CheckPayoutStatus(response.Location);

                // Check if the call is done correct
                if (string.IsNullOrEmpty(statusResponse.errorCode))
                {
                    // Call was maked without any problem
                    Console.WriteLine("Status: " + statusResponse.status);

                }
                else
                {
                    // ERROR
                    Console.WriteLine("CheckPayoutResponse: " + statusResponse.errorCode + " - " + statusResponse.errorMessage);
                }
            }
            else
            {
                // ERROR
                Console.WriteLine("MakePayoutRequest - ERROR: " + response.Error);
            }

            Console.WriteLine(">>> Press enter to exit <<<");
            Console.ReadLine();
        }
    }


}

