using Shared.DTO.Inventory;

namespace Saga.Orchestrator.HttpRepository.Intefaces
{
    public interface IInventoryHttpRepository
    {
        Task<string> CreateSalesOrder(SalesProductDto mdoel);
        Task<bool> DeleteOrderByDocumentNo(string documentNo);
    }
}
