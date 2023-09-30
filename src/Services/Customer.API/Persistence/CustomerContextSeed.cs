using Microsoft.EntityFrameworkCore;

namespace Customer.API.Persistence
{
    public static class CustomerContextSeed
    {
        public static IHost SeedCustomerData(this IHost host)
        {
            using (var scope = host.Services.CreateScope())
            {
                var customerContext = scope.ServiceProvider.GetService<CustomerContext>();
                customerContext.Database.MigrateAsync().GetAwaiter().GetResult();
                CreateCustomer(customerContext, "kelvinlam", "Minh", "Lam", "kelvinlam_1986@yahoo.com.vn").GetAwaiter().GetResult();
                CreateCustomer(customerContext, "nganguyen", "Nga", "Nguyen", "ngagnuyen@yahoo.com.vn").GetAwaiter().GetResult();
                return host;
            }
        }

        private static async Task CreateCustomer(
            CustomerContext customerContext,
            string username, string firstName, string lastName, string email)
        {
            var customer = await customerContext.Customers.SingleOrDefaultAsync(
                x => x.UserName == username || x.Email == email);
            if (customer == null)
            {
                var newCustomer = new Entities.Customer
                {
                    UserName = username,
                    FirstName = firstName,
                    LastName = lastName,
                    Email = email
                };

                await customerContext.Customers.AddAsync(newCustomer);
                await customerContext.SaveChangesAsync();
            }
        }
    }
}
