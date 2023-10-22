using AutoMapper;
using Contracts.Common.Interfaces;
using Infrastructure.Common;
using MediatR;
using Ordering.Application.Common.Intefaces;
using Ordering.Domain.Entities;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, ApiResult<long>>
    {
        private readonly IMapper _mapper;
        private readonly IOrderRepository _orderRepository;

        public CreateOrderCommandHandler(
            IMapper mapper,
            IOrderRepository orderRepository)

        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _orderRepository = orderRepository ?? throw new ArgumentNullException(nameof(orderRepository));
        }

        public async Task<ApiResult<long>> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = _mapper.Map<Order>(request);
            order.DocumentNo = Guid.NewGuid().ToString();
            var orderId = await _orderRepository.CreateAsync(order);
            order.AddedOrder();
            await _orderRepository.SaveChangesAsync();

            return new ApiSuccessResult<long>(order.Id);

        }
    }
}
