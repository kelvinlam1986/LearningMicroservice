using Infrastructure.Extensions;
using Saga.Orchestrator.HttpRepository.Intefaces;
using Shared.DTO.Order;
using Shared.SeedWork;

namespace Saga.Orchestrator.HttpRepository
{
    public class OrderHttpRepository : IOrderHttpRepository
    {
        private readonly HttpClient _client;

        public OrderHttpRepository(HttpClient client)
        {
            _client = client ?? throw new ArgumentNullException(nameof(client));
        }

        public async Task<long> CreateOrder(CreateOrderDto order)
        {
            var response = await _client.PostAsJsonAsync("orders", order);
            if (!response.EnsureSuccessStatusCode().IsSuccessStatusCode)
            {
                return -1;
            }

            var orderId = await response.ReadContentAs<ApiSuccessResult<long>>();
            return orderId.Data;
        }

        public async Task<bool> DeleteOrderByDocumentNo(string documentNo)
        {
            var response = await _client.DeleteAsync($"document-no/{documentNo}");
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteOrer(long id)
        {
            var response = await _client.DeleteAsync($"orders/{id.ToString()}");
            return response.IsSuccessStatusCode;
        }

        public async Task<OrderDto> GetOrder(long id)
        {
            var order = await _client.GetFromJsonAsync<ApiSuccessResult<OrderDto>>($"orders/{id.ToString()}");
            return order.Data;
        }


    }
}
