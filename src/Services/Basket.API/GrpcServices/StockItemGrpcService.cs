using Grpc.Core;
using Grpc.Net.Client;
using Inventory.Grpc.Protos;
using Polly;
using Polly.Retry;
using Shared.Configurations;

namespace Basket.API.GrpcServices
{
    public class StockItemGrpcService
    {
        private readonly ILogger<StockItemGrpcService> _logger;
        private readonly StockProtoService.StockProtoServiceClient _client;
        private readonly GrpcSettings _grpcSetting;
        private readonly AsyncRetryPolicy<StockModel> _retryPolicy;

        public StockItemGrpcService(
            StockProtoService.StockProtoServiceClient client,
            GrpcSettings grpcSettings,
            ILogger<StockItemGrpcService> logger)
        {
            _logger = logger;
            _grpcSetting = grpcSettings;
            _client = client;
            _retryPolicy = Policy<StockModel>.Handle<RpcException>()
                .RetryAsync(3);
        }

        public async Task<StockModel> GetStock(string itemNo)
        {
            try
            {
                var channel = GrpcChannel.ForAddress(_grpcSetting.StockUrl);
                StockProtoService.StockProtoServiceClient client = 
                    new StockProtoService.StockProtoServiceClient(channel);
                _logger.LogInformation($"BEGIN Get Stock {itemNo}");

                return await _retryPolicy.ExecuteAsync(async () =>
                {
                    var result = await client.GetStockAsync(new GetStockRequest { ItemNo = itemNo });
                    if (result != null) 
                    {
                        _logger.LogInformation($"END Get Stock {itemNo} Stock value {result.Quantity}");
                    }
                    return result;
                });
            }
            catch (RpcException ex)
            {
                _logger.LogError($"An error occured at {nameof(StockItemGrpcService)} " +
                    $"Error: {ex.Message}");
                return new StockModel
                {
                    Quantity = -1
                };
            }
        }
    }
}
