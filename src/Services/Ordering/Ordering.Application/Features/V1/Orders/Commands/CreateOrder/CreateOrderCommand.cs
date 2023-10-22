using AutoMapper;
using EventBus.Message.IntegrationEvents.Events;
using MediatR;
using Ordering.Application.Common.Mappings;
using Ordering.Domain.Entities;
using Shared.DTO.Order;
using Shared.SeedWork;

namespace Ordering.Application.Features.V1.Orders.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<ApiResult<long>>, IMapFrom<Order>
    {
        public string UserName { get; set; }
        public decimal TotalPrice { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string EmailAddress { get; set; }
        public string ShippingAddress { get; set; }
        public string InvoiceAddress { get; set; }

        public void Mapping(Profile profile)
        {
            profile.CreateMap<CreateOrderDto, CreateOrderCommand>();
            profile.CreateMap<CreateOrderCommand, Order>();
            profile.CreateMap<BasketCheckoutEvent, CreateOrderCommand>();
        }
    }
}
