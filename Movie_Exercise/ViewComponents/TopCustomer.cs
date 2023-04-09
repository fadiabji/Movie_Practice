using Microsoft.AspNetCore.Mvc;
using Movie_Exercise.Data;
using Movie_Exercise.Models;
using Movie_Exercise.Models.ViewModels;

namespace Movie_Exercise.ViewComponents
{
    public class TopCustomer : ViewComponent
    {
        private readonly ApplicationDbContext _db;
        public TopCustomer(ApplicationDbContext db)
        {
            _db = db;
        }
        public IViewComponentResult Invoke()
        {
            // The Customer who has made the most expensive order (based on the sum of the order rows in an order)
            var listofAllOrders = _db.Orders.Select(o => o).ToList();

            decimal maxOrderTotal = 0;
            Order maxExpensiveOrder = new Order();
            foreach (var order in listofAllOrders)
            {
                // here I will get the order which has a biggest total price
                var minOrderTotal = order.OrderRows.Sum(or => or.Price);
                if (minOrderTotal > maxOrderTotal)
                {
                    maxOrderTotal = minOrderTotal;
                    maxExpensiveOrder = order;
                }
            }
            Customer topCustomer = _db.Customers.Where(c => c.Id == maxExpensiveOrder.CustomerId).FirstOrDefault();
            TopCustomerVM topcutomer = new TopCustomerVM();
            topcutomer.order = maxExpensiveOrder;
            topcutomer.customer = topCustomer;
            return View("Index", topcutomer);
        }

    }
}
