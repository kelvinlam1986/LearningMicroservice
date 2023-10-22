using Infrastructure.Extensions;
using Saga.Orchestrator.HttpRepository.Intefaces;
using Shared.DTO.Inventory;

namespace Saga.Orchestrator.HttpRepository
{
    public class InventoryHttpRepository : IInventoryHttpRepository
    {
        private readonly HttpClient _client;

        public InventoryHttpRepository(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<string> CreateSalesOrder(SalesProductDto model)
        {
            var response = await _client.PostAsJsonAsync<SalesProductDto>($"inventory/sales/{model.ItemNo}", model);
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode == false)
            {
                throw new Exception($"Create Sales Order For ItemNo: {model.ItemNo} not success");
            }

            var inventory = await response.ReadContentAs<InventoryEntryDto>();
            return inventory.DocumentNo;
        }

        public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
        {
            var response = await _client.DeleteAsync($"inventory/document-no/{documentNo}");
            if (response.EnsureSuccessStatusCode().IsSuccessStatusCode == false)
            {
                throw new Exception($"Delete Order For Document No: {documentNo} not success");
            }

            var result = await response.ReadContentAs<bool>();
            return result;

        }
    }
}
