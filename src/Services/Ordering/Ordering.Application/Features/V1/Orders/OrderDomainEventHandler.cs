using Contracts.Services;
using MediatR;
using Ordering.Application.Common.Intefaces;
using Ordering.Application.Features.V1.Orders.Commands.CreateOrder;
using Ordering.Domain.OrderAggregate.Events;
using Serilog;
using Shared.Services.Email;

namespace Ordering.Application.Features.V1.Orders
{
    public class OrderDomainEventHandler :
        INotificationHandler<OrderCreatedEvent>,
        INotificationHandler<OrderDeletedEvent>
    {
        private readonly ILogger _logger;
        private readonly ISmtpEmailService _emailService;
        private readonly IOrderRepository _orderRepository;

        public OrderDomainEventHandler(
            ISmtpEmailService emailService,
            ILogger logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        public async Task Handle(OrderCreatedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Ordering domain event hander. Domain Event {DomainEvent}", notification.GetType().Name);
            _logger.Information($"Prepare to send email to customer. OrderId: {notification.Id}");

            var message = new MailRequest
            {
                Body = $"Thanks for order Kelvin Product. {notification.FirstName} {notification.LastName}. Your order {notification.Id}",
                Subject = $"Thanks for order Kelvin Product. {notification.FirstName} {notification.LastName}",
                ToAddress = notification.EmailAddress
            };

            await _emailService.SendEmailAsync(message);
        }

        public Task Handle(OrderDeletedEvent notification, CancellationToken cancellationToken)
        {
            _logger.Information("Ordering domain event hander. Domain Event {DomainEvent}", notification.GetType().Name);
            return Task.CompletedTask;
        }
    }
}
