using Shared.DTO.Order;

namespace Saga.Orchestrator.HttpRepository.Intefaces
{
    public interface IOrderHttpRepository
    {
        Task<long> CreateOrder(CreateOrderDto order);
        Task<OrderDto> GetOrder(long id);

        Task<bool> DeleteOrer(long id);
        Task<bool> DeleteOrderByDocumentNo(string documentNo);

    }
}
