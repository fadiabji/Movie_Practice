using Movie_Exercise.Data;

namespace Movie_Exercise.Services
{
    public class CustomerService : ICustomerService
    {
        
        private readonly ApplicationDbContext _db;
        public CustomerService(ApplicationDbContext db)
        {
            _db = db;
        }


    }
}
