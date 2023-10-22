using AutoMapper;
using Saga.Orchestrator.HttpRepository.Intefaces;
using Saga.Orchestrator.Services.Interfaces;
using Shared.DTO.Baskets;
using Shared.DTO.Inventory;
using Shared.DTO.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.Services
{
    public class CheckoutService : ICheckoutService
    {
        private readonly IOrderHttpRepository _orderHttpRepository;
        private readonly IBasketHttpRepository _basketHttpRepository;
        private readonly IInventoryHttpRepository _inventoryHttpRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public CheckoutService(
            IOrderHttpRepository orderHttpRepository, 
            IBasketHttpRepository basketHttpRepository, 
            IInventoryHttpRepository inventoryHttpRepository,
            IMapper mapper,
            ILogger logger)
        {
            _orderHttpRepository = orderHttpRepository ?? throw new ArgumentNullException(nameof(orderHttpRepository));
            _basketHttpRepository = basketHttpRepository ?? throw new ArgumentNullException(nameof(basketHttpRepository));
            _inventoryHttpRepository = inventoryHttpRepository ?? throw new ArgumentNullException(nameof(inventoryHttpRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<bool> CheckoutOrder(string username, BasketCheckoutDto basketCheckout)
        {
            // Get cart from BasketHttpRepository
            _logger.Information($"Start: Get Cart {username}");

            var cart = await _basketHttpRepository.GetBasket(username);
            if (cart == null)
            {
                return false;
            }

            _logger.Information($"End Get Cart {username} success");

            // Create order from OrderHttpRepository
            _logger.Information("$Start Create Order");
            var order = _mapper.Map<CreateOrderDto>(basketCheckout);
            order.TotalPrice = basketCheckout.TotalPrice;
            var orderId = await _orderHttpRepository.CreateOrder(order);
            if (orderId < 0)
            {
                return false;
            }

            // Get Order by order id
            var addedOrder = await _orderHttpRepository.GetOrder(orderId);
            _logger.Information($"End Create order success, orderId = {orderId} DocumentNo = {addedOrder.DocumentNo}");

            var inventoryDocumentNos = new List<string>();
            bool result;
            try
            {
                // Sales Items from InventoryHttpRepository
                foreach (var item in cart.Items)
                {
                    _logger.Information($"Sales Item No {item.ItemNo} Quantity {item.Quantity}");
                    var salesOrder = new SalesProductDto(addedOrder.DocumentNo, item.Quantity);
                    salesOrder.SetItemNo(item.ItemNo);
                    var documentNo = await _inventoryHttpRepository.CreateSalesOrder(salesOrder);
                    inventoryDocumentNos.Add(documentNo);
                    _logger.Information($"End Sales Item No {item.ItemNo} Quantity {item.Quantity} DocumentNo {documentNo}");
                }


                result = await _basketHttpRepository.DeleteBasket(username);
            }
            catch (Exception ex)
            {
                _logger.Error(ex.Message);
                await RollbackCheckoutOrder(username, addedOrder.Id, inventoryDocumentNos);
                result = false;
            }

            return result;
        }


        private async Task RollbackCheckoutOrder(string username, long orderId, List<string> inventoryDocumentNos)
        {
            _logger.Information($"Start: RollbackCheckoutOrder for username: {username}, " +
                $"order id: {orderId} " +
                $"inventory no: {string.Join(", ", inventoryDocumentNos)}");

            var deletedDocumentNos = new List<string>();

            _logger.Information($"Start Delete Order Id={orderId}");
            await _orderHttpRepository.DeleteOrer(orderId);
            _logger.Information($"End Delete Order Id={orderId}");

            // delete order by order id, order's document no
            foreach (var documentNo in inventoryDocumentNos)
            {
                await _inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo);
                deletedDocumentNos.Add(documentNo);
            }

            _logger.Information($"End Deleted Document Nos {string.Join(", ", deletedDocumentNos)}");

        }
    }
}
