using Inventory.Product.API.Entities;
using MongoDB.Driver;
using Shared.Configurations;

namespace Inventory.Product.API.Persistence
{
    public class InventoryDbSeed
    {
        public async Task SeedDataAsync(IMongoClient mongoClient, MongoDbSettings settings)
        {
            var databaseName = settings.DatabaseName;
            var database = mongoClient.GetDatabase(databaseName);
            var inventoryCollection = database.GetCollection<InventoryEntry>("InventoryEntries");
            if (await inventoryCollection.EstimatedDocumentCountAsync() == 0)
            {
                await inventoryCollection.InsertManyAsync(GetPreconfiguredInventoryEntry());
            }
        } 

        private IEnumerable<InventoryEntry> GetPreconfiguredInventoryEntry()
        {
            return new List<InventoryEntry>()
            {
                new InventoryEntry
                {
                    ItemNo = "P1",
                    DocumentNo = Guid.NewGuid().ToString(),
                    Quantity = 10,
                    ExternalDocumentNo = Guid.NewGuid().ToString(),
                    DocumentType = Shared.Enums.Inventory.EDocumentType.Purchase
                },
                new InventoryEntry
                {
                    ItemNo = "P2",
                    DocumentNo = Guid.NewGuid().ToString(),
                    Quantity = 20,
                    ExternalDocumentNo = Guid.NewGuid().ToString(),
                    DocumentType = Shared.Enums.Inventory.EDocumentType.Purchase
                },
            };
        }

    }
}
