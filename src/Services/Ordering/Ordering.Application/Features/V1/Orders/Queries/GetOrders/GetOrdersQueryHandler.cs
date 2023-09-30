using AutoMapper;
using MediatR;
using Ordering.Application.Common.Intefaces;
using Ordering.Application.Common.Models;
using Ordering.Application.Features.V1.Orders.Queries.GetOrders;
using Shared.SeedWork;
using ILogger = Serilog.ILogger;

namespace Ordering.Application.Features.V1.Orders
{
    public class GetOrdersQueryHandler : IRequestHandler<GetOrdersQuery, ApiResult<List<OrderDto>>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger _logger;

        public GetOrdersQueryHandler(
            IMapper mapper, 
            IOrderRepository orderRepository,
            ILogger logger)
        {
            _mapper = mapper ?? throw new ArgumentException(nameof(mapper));
            _orderRepository = orderRepository ?? throw new ArgumentException(nameof(orderRepository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<ApiResult<List<OrderDto>>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            _logger.Information($"GetOrdersQueryHandler BEGIN Username - {request.UserName}");
            var orders = await _orderRepository.GetOrdersByUserName(request.UserName);
            var result = _mapper.Map<List<OrderDto>>(orders);

            _logger.Information($"GetOrdersQueryHandler END Username - {request.UserName}");
            return new ApiSuccessResult<List<OrderDto>>(result);
        }
    }
}
