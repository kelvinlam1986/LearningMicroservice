using AutoMapper;
using Shared.DTO.Baskets;
using Shared.DTO.Order;

namespace Saga.Orchestrator
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<BasketCheckoutDto, CreateOrderDto>();
        }
    }
}
