using Grpc.Net.Client;
using Inventory.Grpc.Protos;
using Shared.Configurations;

namespace Basket.API.GrpcServices
{
    public class StockItemGrpcService
    {
        private readonly ILogger<StockItemGrpcService> _logger;
        private readonly StockProtoService.StockProtoServiceClient _client;
        private readonly GrpcSettings _grpcSetting;

        public StockItemGrpcService(
            StockProtoService.StockProtoServiceClient client,
            GrpcSettings grpcSettings,
            ILogger<StockItemGrpcService> logger)
        {
            _logger = logger;
            _grpcSetting = grpcSettings;
            _client = client;
        }

        public async Task<StockModel> GetStock(string itemNo)
        {
            try
            {
                var channel = GrpcChannel.ForAddress("http://inventory.grpc");
                StockProtoService.StockProtoServiceClient client = 
                    new StockProtoService.StockProtoServiceClient(channel);
                _logger.LogInformation($"BEGIN Get Stock {itemNo}");
                var result = await client.GetStockAsync(new GetStockRequest { ItemNo = itemNo });

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured at {nameof(StockItemGrpcService)} " +
                    $"Error: {ex.Message}");
                throw ex;
            }
        }
    }
}
