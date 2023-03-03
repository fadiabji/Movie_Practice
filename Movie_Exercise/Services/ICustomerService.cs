using Movie_Exercise.Models;

namespace Movie_Exercise.Services
{
    public interface ICustomerService
    {
        Task<List<Customer>> GetAllCustomer();

        Task<Customer> GetCustomerById(int? id);

        void AddCustomer(Customer newCustomer);

        void UpdateCustomer(Customer customer);

        void RemoveCustomer(Customer customer);

        bool IsExists(int? id);

    }
}
