using Movie_Exercise.Data;
using Movie_Exercise.Models;

namespace Movie_Exercise.Services
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _db;
        public OrderService(ApplicationDbContext db)
        {
            _db = db;            
        }

        public void AddOrder(Order order)
        {
            _db.Orders.Add(order);
            _db.SaveChanges();
        }
        
        public List<Order> GetAllOrders() 
        {
            return _db.Orders.ToList();
        }

        public List<Order> GetOrderByCustomerId(int customerId)
        {
            return _db.Orders.Where(o =>o.CustomerId == customerId).ToList();
        }

    }
}
