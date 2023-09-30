using Contracts.Common.Interfaces;
using Infrastructure.Common.Models;
using Inventory.Product.API.Entities;
using Shared.DTO.Inventory;

namespace Inventory.Product.API.Services.Interface
{
    public interface IInventoryService : IMongoRepositoryBase<InventoryEntry>
    {
        Task<IEnumerable<InventoryEntryDto>> GetAllByItemNo(string itemNo);

        Task<PagedList<InventoryEntryDto>> GetAllByItemNoPaging(GetInventoryPagingQuery query);

        Task<InventoryEntryDto> GetByIdAsync(string id);

        Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model);
    }
}
