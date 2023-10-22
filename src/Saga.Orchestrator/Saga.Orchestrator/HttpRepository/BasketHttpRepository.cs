using Saga.Orchestrator.HttpRepository.Intefaces;
using Shared.DTO.Baskets;

namespace Saga.Orchestrator.HttpRepository
{
    public class BasketHttpRepository : IBasketHttpRepository
    {
        private readonly HttpClient _client;

        public BasketHttpRepository(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<bool> DeleteBasket(string username)
        {
            var response = await _client.DeleteAsync($"baskets/{username}");
            return response.EnsureSuccessStatusCode().IsSuccessStatusCode;
        }

        public async Task<CartDto> GetBasket(string username)
        {
            var cart = await _client.GetFromJsonAsync<CartDto>($"baskets/{username}");
            if (cart == null || !cart.Items.Any()) 
            {
                return null;
            }

            return cart;
        }
    }
}
