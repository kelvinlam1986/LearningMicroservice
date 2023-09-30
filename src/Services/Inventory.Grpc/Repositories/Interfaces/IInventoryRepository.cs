using Contracts.Common.Interfaces;
using Inventory.Grpc.Entities;

namespace Inventory.Grpc.Repositories.Interfaces
{
    public interface IIventoryRepository : IMongoRepositoryBase<InventoryEntry>
    {
        Task<int> GetStockQuantity(string itemNo);
    }
}
