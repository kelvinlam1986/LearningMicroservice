using Contracts.Common.Interfaces;
using Customer.API.Persistence;

namespace Customer.API.Repositories.Interface
{
    public interface ICustomerRepository : IRepositoryQueryBaseAsync<Entities.Customer, int, CustomerContext>
    {
        Task<Entities.Customer> GetCustomerByUserName(string username);
        Task<IEnumerable<Entities.Customer>> GetCustomers();
    }
}
