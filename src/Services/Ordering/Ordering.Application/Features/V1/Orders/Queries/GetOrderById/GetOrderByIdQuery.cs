using MediatR;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<ApiResult<Ordering.Application.Common.Models.OrderDto>>
    {
        public long Id { get; private set; }

        public GetOrderByIdQuery(long id)
        {
            Id = id;
        }
    }
}
