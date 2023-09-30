using AutoMapper;
using Customer.API.DTOs;
using Customer.API.Repositories.Interface;

namespace Customer.API.Controllers
{
    public static class CustomerController
    {
        public static void MapCustomerApi(this WebApplication app)
        {
            app.MapGet("/api/customers/{username}", async (string username, ICustomerRepository customerRepository, IMapper mapper)
            =>
            {
                var customer = await customerRepository.GetCustomerByUserName(username);
                if (customer == null)
                {
                    return Results.NotFound();
                }

                var result = mapper.Map<CustomerDto>(customer);

                return Results.Ok(result);
            });
        }
    }
}
