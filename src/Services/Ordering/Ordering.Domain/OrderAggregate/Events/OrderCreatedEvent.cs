using Contracts.Common.Events;
using Ordering.Domain.Enums;

namespace Ordering.Domain.OrderAggregate.Events
{
    public class OrderCreatedEvent : BaseEvent
    {
        public long Id { get; private set; }
        public string UserName { get; private set; }
        public decimal TotalPrice { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string EmailAddress { get; private set; }
        public string ShippingAddress { get; private set; }
        public string InvoiceAddress { get; private set; }
        public OrderStatus Status { get; private set; }

        public OrderCreatedEvent(
            long id,
            string username,
            decimal totalPrice,
            string firstName,
            string lastName,
            string emailAddress,
            string shippingAddress,
            string invoiceAddress,
            OrderStatus status)
        {
            Id = id;
            UserName = username;
            TotalPrice = totalPrice;
            FirstName = firstName;
            LastName = lastName;
            EmailAddress = emailAddress;
            ShippingAddress = shippingAddress;
            InvoiceAddress = invoiceAddress;
            Status = status;
        }
    }
}
