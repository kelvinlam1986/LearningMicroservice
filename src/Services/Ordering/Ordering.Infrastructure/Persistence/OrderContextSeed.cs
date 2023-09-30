using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace Ordering.Infrastructure.Persistence
{
    public class OrderContextSeed
    {
        private readonly OrderContext _orderContext;
        private readonly ILogger _logger;

        public OrderContextSeed(OrderContext orderContext, ILogger logger)
        {
            _orderContext = orderContext;
            _logger = logger;
        }

        public async Task InitialiseAsync()
        {
            try
            {
                if (_orderContext.Database.IsSqlServer())
                {
                    await _orderContext.Database.MigrateAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured in initialise order context seed");
                throw;
            }
        }

        public async Task SeedAsync()
        {
            try
            {
                await TrySeedAsync();
                await _orderContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "An error occured when seed database");
                throw;
            }
        }

        public async Task TrySeedAsync()
        {
            if (!await _orderContext.Orders.AnyAsync())
            {
                await _orderContext.Orders.AddRangeAsync(new Domain.Entities.Order
                {
                    UserName = "kelvinlam",
                    EmailAddress = "kelvincoder@gmail.com",
                    FirstName = "Kelvin",
                    LastName = "Lam",
                    InvoiceAddress = "240/2 Le Thanh Ton Street, District 1, Ben Thanh Ward",
                    ShippingAddress = "240/2 Le Thanh Ton Street, District 1, Ben Thanh Ward",
                    TotalPrice = 100000,
                    CreatedDate = DateTime.UtcNow,
                    LastModifiedDate = DateTime.UtcNow
                });
            }
        }
    }
}
