using Grpc.Core;
using Inventory.Grpc.Protos;
using Inventory.Grpc.Repositories.Interfaces;
using ILogger = Serilog.ILogger;

namespace Inventory.Grpc.Services
{
    public class InventoryService : StockProtoService.StockProtoServiceBase
    {
        private readonly IIventoryRepository _repository;
        private readonly ILogger _logger;

        public InventoryService(IIventoryRepository repository, ILogger logger)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }


        public override async Task<StockModel> GetStock(GetStockRequest request, ServerCallContext context)
        {
            _logger.Information($"BEGIN Get stock by Item No {request.ItemNo}");

            var quantity = await _repository.GetStockQuantity(request.ItemNo);
            var result = new StockModel { Quantity = quantity };

            _logger.Information($"BEGIN Get stock by Item No {request.ItemNo} Quantity = {result.Quantity}");

            return result;
        }

    }
}
