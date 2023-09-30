using Inventory.Grpc.Entities;
using Inventory.Grpc.Repositories.Interfaces;
using Inventory.Product.API.Repositories.Abstraction;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Grpc.Repositories
{
    public class InventoryRepository : MongoRepository<InventoryEntry>, IIventoryRepository
    {
        public InventoryRepository(IMongoClient client, MongoDbSettings settings) : base(client, settings)
        {
        }

        public async Task<int> GetStockQuantity(string itemNo)
        {
            return Collection.AsQueryable()
                .Where(x => x.ItemNo == itemNo)
                .Sum(x => x.Quantity);
        }
    }
}
