using Movie_Exercise.Models;

namespace Movie_Exercise.Services
{
    public interface IOrderService
    {

        public void AddOrder(Order order);

        public List<Order> GetAllOrders();

        public List<Order> GetOrderByCustomerId(int customerId);


    }
}
