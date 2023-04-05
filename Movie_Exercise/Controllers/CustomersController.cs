using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Movie_Exercise.Data;
using Movie_Exercise.Models;
using Movie_Exercise.Services;

namespace Movie_Exercise.Controllers
{
    public class CustomersController : Controller
    {

        private readonly ICustomerService _customerService;
        private readonly IOrderService _orderService;

        public CustomersController(ICustomerService customerService,IOrderService orderService)
        {
            _customerService = customerService;
            _orderService = orderService;
        }
        public async Task<IActionResult> Index()
        {
            return View(await Task.Run(() => _customerService.GetAllCustomer()));
        }

        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _customerService.GetAllCustomer() == null)
            {
                return NotFound();
            }

            var movie = await Task.Run(() => _customerService.GetCustomerById(id));
            if (movie == null)
            {
                return NotFound();
            }

            return View(movie);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                await Task.Run(() => _customerService.AddCustomer(customer));
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // GET: Customer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _customerService.GetAllCustomer() == null)
            {
                return NotFound();
            }

            var customer = await Task.Run(() => _customerService.GetCustomerById(id));

            if (customer == null)
            {
                return NotFound();
            }
            return View(customer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Customer customer, IFormFile file)
        {
            if (id != customer.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await Task.Run(() => _customerService.UpdateCustomer(customer));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CustomerExists(customer.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _customerService.GetAllCustomer() == null)
            {
                return NotFound();
            }

            var customer = await Task.Run(() => _customerService.GetCustomerById(id));
            if (customer == null)
            {
                return NotFound();
            }

            return View(customer);
        }

        // POST: Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_customerService.GetAllCustomer() == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Customer'  is null.");
            }
            var customer = await Task.Run(() => _customerService.GetCustomerById(id));

            if (await Task.Run(() => _customerService.IsExists(id)))
            {
                await Task.Run(() => _customerService.RemoveCustomer(customer));
            }
            return RedirectToAction(nameof(Index));
        }

    
        // Retrun all orders for a given customer
        
        public  IActionResult Orders(string email)
        {
            //// bring me customerId
            int customerId = _customerService.GetCustomerByEmail(email).Id;
            // Give me all orders for this customers
            var listOfOrders = _orderService.GetOrderByCustomerId(customerId).ToList();
            
            if (listOfOrders.Any())
            {
                return View(listOfOrders);
            }
            else
            {
                return NotFound();
            }
            
        }



        private bool CustomerExists(int id)
        {
            return _customerService.IsExists(id);
        }

    }
}
