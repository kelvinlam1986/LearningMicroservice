using AutoMapper;
using Infrastructure.Common.Models;
using Infrastructure.Extensions;
using Inventory.Product.API.Entities;
using Inventory.Product.API.Repositories.Abstraction;
using Inventory.Product.API.Services.Interface;
using MongoDB.Bson;
using MongoDB.Driver;
using Shared.Configurations;
using Shared.DTO.Inventory;

namespace Inventory.Product.API.Services
{
    public class InventoryService : MongoRepository<InventoryEntry>, IInventoryService
    {
        private readonly IMapper _mapper; 

        public InventoryService(IMongoClient client, MongoDbSettings settings, IMapper mapper) : base(client, settings)
        {
            _mapper = mapper;
        }

        public async Task DeleteByDocumentNoAsync(string documentNo)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, documentNo);
            await Collection.DeleteOneAsync(filter);
        }

        public async Task<IEnumerable<InventoryEntryDto>> GetAllByItemNo(string itemNo)
        {
            var entities = await FindAll().Find(x => x.ItemNo == itemNo)
                .ToListAsync();

            var result = _mapper.Map<IEnumerable<InventoryEntryDto>>(entities);
            return result;
        }

        public async Task<PagedList<InventoryEntryDto>> GetAllByItemNoPaging(GetInventoryPagingQuery query)
        {
            var filterSearchTerm = Builders<InventoryEntry>.Filter.Empty;
            var filterItemNo = Builders<InventoryEntry>.Filter.Eq(x => x.ItemNo, query.ItemNo());
            if (!string.IsNullOrEmpty(query.SearchTerm))
            {
                filterSearchTerm = Builders<InventoryEntry>.Filter.Eq(x => x.DocumentNo, query.SearchTerm);
            }

            var andFilter = filterItemNo & filterSearchTerm;
            var pagedList = await Collection.PaginatedListAsync(
                andFilter, pageIndex: query.PageNumber, pageSize: query.PageSize);

            var items = _mapper.Map<IEnumerable<InventoryEntryDto>>(pagedList);
            var result = new PagedList<InventoryEntryDto>(items, pagedList.MetaData.TotalItems,
                query.PageNumber, query.PageSize);
            return result;

        }

        public async Task<InventoryEntryDto> GetByIdAsync(string id)
        {
            FilterDefinition<InventoryEntry> filter = Builders<InventoryEntry>.Filter.Eq(x => x.Id, id);
            var entity = await FindAll().Find(filter).FirstOrDefaultAsync();
            var result = _mapper.Map<InventoryEntryDto>(entity);
            return result;
        }

        public async Task<InventoryEntryDto> PurchaseItemAsync(string itemNo, PurchaseProductDto model)
        {
            var entityToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                DocumentNo = model.DocumentNo,
                Quantity = model.Quantity,
                DocumentType = model.DocumentType
            };

            await CreateAsync(entityToAdd);
            var result = _mapper.Map<InventoryEntryDto>(entityToAdd);
            return result;
        }

        public async Task<InventoryEntryDto> SalesItemAsync(string itemNo, SalesProductDto model)
        {
            var entityToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
            {
                ItemNo = itemNo,
                ExternalDocumentNo = model.ExternalDocumentNo,
                Quantity = model.Quantity * -1,
                DocumentType = model.DocumentType,
                DocumentNo = model.ExternalDocumentNo
            };

            await CreateAsync(entityToAdd);
            var result = _mapper.Map<InventoryEntryDto>(entityToAdd);
            return result;
        }

        public async Task<string> SalesOrderAsync(SalesOrderDto model)
        {
            var documentNo = Guid.NewGuid().ToString();
            foreach (var salesItem in model.SalesItems)
            {
                var entityToAdd = new InventoryEntry(ObjectId.GenerateNewId().ToString())
                {
                    ItemNo = salesItem.ItemNo,
                    ExternalDocumentNo = model.OrderNo,
                    Quantity = salesItem.Quantity * -1,
                    DocumentType = salesItem.DocumentType,
                    DocumentNo = documentNo
                };

                await CreateAsync(entityToAdd);
            }

            return documentNo;
        }
    }
}
