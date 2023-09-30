using AutoMapper;
using Basket.API.DTO;
using Basket.API.Entities;
using EventBus.Message.IntegrationEvents.Events;

namespace Basket.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<BasketCheckoutDto, BasketCheckoutEvent>();
        }
    }
}
