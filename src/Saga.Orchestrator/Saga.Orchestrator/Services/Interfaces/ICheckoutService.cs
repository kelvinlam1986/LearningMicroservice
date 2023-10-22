using Shared.DTO.Baskets;

namespace Saga.Orchestrator.Services.Interfaces
{
    public interface ICheckoutService
    {
        Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckout);
    }
}
