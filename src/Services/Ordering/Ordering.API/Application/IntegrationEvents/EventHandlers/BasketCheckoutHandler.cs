using AutoMapper;
using EventBus.Message.IntegrationEvents.Events;
using MassTransit;
using MediatR;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using ILogger = Serilog.ILogger;

namespace Ordering.API.Application.IntegrationEvents.EventHandlers
{
    public class BasketCheckoutHandler : IConsumer<BasketCheckoutEvent>
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public BasketCheckoutHandler(IMediator mediator, IMapper mapper, ILogger logger)
        {
            _mediator = mediator;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task Consume(ConsumeContext<BasketCheckoutEvent> context)
        {
            var command = _mapper.Map<CreateOrderCommand>(context.Message);
            var result = await _mediator.Send(command);
            _logger.Information($"Basket checkout event consumed successfully " +
                "Order created with new id {newOrderId}", result.Data);
        }
    }
}
