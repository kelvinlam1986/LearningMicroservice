using AutoMapper;
using Customer.API.DTOs;

namespace Customer.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Entities.Customer, CustomerDto>();
        }
    }
}
