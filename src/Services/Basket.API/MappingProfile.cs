using AutoMapper;
using Basket.API.DTO;
using Basket.API.Entities;
using EventBus.Message.IntegrationEvents.Events;
using Shared.DTO.Baskets;

namespace Basket.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<BasketCheckoutDto, BasketCheckoutEvent>();
            CreateMap<CartDto, Cart>().ReverseMap();
            CreateMap<CartItemDto, CartItem>().ReverseMap();
        }
    }
}
