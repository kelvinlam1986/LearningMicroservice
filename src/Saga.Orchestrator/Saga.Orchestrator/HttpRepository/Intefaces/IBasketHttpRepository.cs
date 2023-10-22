using Shared.DTO.Baskets;

namespace Saga.Orchestrator.HttpRepository.Intefaces
{
    public interface IBasketHttpRepository
    {
        Task<CartDto> GetBasket(string username);
        Task<bool> DeleteBasket(string username);

    }
}
