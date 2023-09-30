using AutoMapper;
using Inventory.Product.API.Entities;
using Shared.DTO.Inventory;

namespace Inventory.Product.API
{
    public class MappingProfile : Profile
    {
        public MappingProfile() 
        {
            CreateMap<InventoryEntry, InventoryEntryDto>();
            CreateMap<PurchaseProductDto, InventoryEntryDto>();
        }
    }
}
