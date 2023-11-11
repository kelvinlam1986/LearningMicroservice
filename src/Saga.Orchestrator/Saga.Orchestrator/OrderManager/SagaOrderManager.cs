using AutoMapper;
using Contracts.Sagas.OrderManager;
using Saga.Orchestrator.HttpRepository.Intefaces;
using Shared.DTO.Baskets;
using Shared.DTO.Inventory;
using Shared.DTO.Order;
using ILogger = Serilog.ILogger;

namespace Saga.Orchestrator.OrderManager
{
    public class SagaOrderManager : ISagaOrderManager<BasketCheckoutDto, OrderResponse>
    {
        private readonly IOrderHttpRepository _orderHttpRepository;
        private readonly IBasketHttpRepository _basketHttpRepository;
        private readonly IInventoryHttpRepository _inventoryHttpRepository;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;

        public SagaOrderManager(
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

        public OrderResponse CreateOrder(BasketCheckoutDto input)
        {
            var orderMachine = new Stateless.StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.NotStarted);

            long orderId = -1;
            CartDto cart = null;
            OrderDto addedOrder = null;
            string? inventoryDocumentNo = "";

            orderMachine.Configure(EOrderTransactionState.NotStarted)
                .PermitDynamic(EOrderAction.GetBasket, () =>
                {
                    cart = _basketHttpRepository.GetBasket(input.UserName).Result;
                    return cart != null ? EOrderTransactionState.BasketGot : EOrderTransactionState.BasketGetFailed;
                });

            orderMachine.Configure(EOrderTransactionState.BasketGot)
                .PermitDynamic(EOrderAction.CreateOrder, () =>
                {
                    var order = _mapper.Map<CreateOrderDto>(input);
                    order.TotalPrice = cart.TotalPrice;
                    orderId = _orderHttpRepository.CreateOrder(order).Result;
                    return orderId > 0 ? EOrderTransactionState.OrderCreated : EOrderTransactionState.OrderCreatedFailed;
                }).OnEntry(() => orderMachine.Fire(EOrderAction.CreateOrder));

            orderMachine.Configure(EOrderTransactionState.OrderCreated)
                .PermitDynamic(EOrderAction.GetOrder, () =>
                {
                    addedOrder = _orderHttpRepository.GetOrder(orderId).Result;
                    return addedOrder != null ? EOrderTransactionState.OrderGot : EOrderTransactionState.OrdertGetFailed;
                }).OnEntry(() => orderMachine.Fire(EOrderAction.GetOrder));

            orderMachine.Configure(EOrderTransactionState.OrderGot)
                .PermitDynamic(EOrderAction.UpdateInventory, () =>
                {
                    var salesOrder = new SalesOrderDto
                    {
                        OrderNo = addedOrder.DocumentNo,
                        SalesItems = _mapper.Map<List<SalesItemDto>>(cart.Items)
                    };

                    inventoryDocumentNo = _inventoryHttpRepository.CreateOrderSales(addedOrder.DocumentNo, salesOrder).Result;
                    return inventoryDocumentNo != null ? EOrderTransactionState.InventoryUpdated : EOrderTransactionState.InventoryUpdateFailed;
                }).OnEntry(() => orderMachine.Fire(EOrderAction.UpdateInventory));

            orderMachine.Configure(EOrderTransactionState.InventoryUpdated)
                .PermitDynamic(EOrderAction.DeleteBasket, () =>
                {
                    var result = _basketHttpRepository.DeleteBasket(input.UserName).Result;
                    return result ? EOrderTransactionState.BasketDeleted : EOrderTransactionState.InventoryUpdateFailed;
                }).OnEntry(() => orderMachine.Fire(EOrderAction.DeleteBasket));

            orderMachine.Configure(EOrderTransactionState.InventoryUpdateFailed)
                .PermitDynamic(EOrderAction.DeleteInventory, () =>
                {
                    RollbackOrder(input.UserName, inventoryDocumentNo, orderId);
                    return EOrderTransactionState.InventoryRollback;
                });

            orderMachine.Fire(EOrderAction.GetBasket);

            return new OrderResponse(orderMachine.State == EOrderTransactionState.BasketDeleted);

        }

        public OrderResponse RollbackOrder(string username, string documentNo, long orderId)
        {
            var orderStateMachine = new Stateless.StateMachine<EOrderTransactionState, EOrderAction>(EOrderTransactionState.RollbackInventory);

            orderStateMachine.Configure(EOrderTransactionState.RollbackInventory)
                .PermitDynamic(EOrderAction.DeleteInventory, () =>
                {
                    var result =_inventoryHttpRepository.DeleteOrderByDocumentNo(documentNo).Result;
                    return EOrderTransactionState.InventoryRollback;
                });

            orderStateMachine.Configure(EOrderTransactionState.InventoryRollback)
              .PermitDynamic(EOrderAction.DeleteOrder, () =>
              {
                  var result = _orderHttpRepository.DeleteOrer(orderId).Result;
                  return result ? EOrderTransactionState.OrderDeleted : EOrderTransactionState.OrderDeletedFailed;
              }).OnEntry(() => orderStateMachine.Fire(EOrderAction.DeleteOrder));


            orderStateMachine.Fire(EOrderAction.DeleteInventory);

            return new OrderResponse(orderStateMachine.State == EOrderTransactionState.InventoryRollback);
        }
    }
}
