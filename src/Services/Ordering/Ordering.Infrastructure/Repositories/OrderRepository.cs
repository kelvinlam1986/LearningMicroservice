using Contracts.Common.Interfaces;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Ordering.Application.Common.Intefaces;
using Ordering.Domain.Entities;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository : RepositoryBaseAsync<Order, long, OrderContext>, IOrderRepository
    {
        public OrderRepository(OrderContext context, IUnitOfWork<OrderContext> unitOfWork) 
            : base(context, unitOfWork)
        {
        }

        public async Task<Order> GetOrderByDocumentNo(string documentNo)
        {
            var order = await FindByCondition(x => x.DocumentNo == documentNo).FirstOrDefaultAsync();
            return order;
        }

        public async Task<IEnumerable<Order>> GetOrdersByUserName(string userName)
        {
            var orders = await FindByCondition(x => x.UserName == userName).ToListAsync();
            return orders;
        }
    }
}
