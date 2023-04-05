using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Movie_Exercise.Data;
using Movie_Exercise.Models;
using System.Diagnostics.CodeAnalysis;

namespace Movie_Exercise.Services
{
    public class CustomerService : ICustomerService
    {
        
        private readonly ApplicationDbContext _db;
        private readonly IOrderService _orderService;
        public CustomerService(ApplicationDbContext db, IOrderService orderService )
        {
            _db = db;
            _orderService = orderService;
        }
        
        public async Task<List<Customer>> GetAllCustomer()
        {
            return await Task.Run(() => _db.Customers.ToList());
        }

        public async Task<Customer> GetCustomerById(int? id)
        {
            return await Task.Run(() => _db.Customers.AsNoTracking().FirstOrDefault(c => c.Id == id));
        }

        public void AddCustomer(Customer newCustomer)
        {
            _db.Add(newCustomer);
            _db.SaveChanges();
        }

        public void UpdateCustomer(Customer customer)
        {
            //_db.Customers.Attach(customer);
            _db.Update(customer);
            _db.SaveChanges();
        }


        public void RemoveCustomer(Customer customer)
        {
            _db.Remove(customer);
            _db.SaveChanges();
        }

        public Customer GetCustomerByEmail(string email)
        {
            return  _db.Customers.AsNoTracking().FirstOrDefault(c => c.EmailAddress == email);
        }
       
        public List<Order> GetCustomerOrders(int customerId) 
        {
            return _db.Orders.Select(o => o).Where(o => o.CustomerId == customerId).ToList(); 
        }

        public bool IsExists(int? id)
        {
            return _db.Customers.Any(e => e.Id == id);
        }


    }
}
