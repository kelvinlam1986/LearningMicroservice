using Contracts.Common.Interfaces;
using Customer.API.Persistence;
using Customer.API.Repositories.Interface;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;

namespace Customer.API.Repositories
{
    public class CustomerRepository : RepositoryBaseAsync<Entities.Customer, int, CustomerContext>, ICustomerRepository
    {
        public CustomerRepository(CustomerContext context, IUnitOfWork<CustomerContext> unitOfWork) 
            : base(context, unitOfWork)
        {
        }

        public async Task<Entities.Customer> GetCustomerByUserName(string username)
        {
            return await FindByCondition(x => x.UserName == username).FirstOrDefaultAsync();
        }

        public  async Task<IEnumerable<Entities.Customer>> GetCustomers()
        {
            return await FindAll().ToListAsync();
        }
    }
}
