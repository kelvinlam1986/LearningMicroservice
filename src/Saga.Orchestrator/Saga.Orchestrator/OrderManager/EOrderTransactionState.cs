namespace Saga.Orchestrator.OrderManager
{
    public enum EOrderTransactionState
    {
        NotStarted,
        BasketGot,
        BasketGetFailed,
        OrderCreated,
        OrderCreatedFailed,
        OrderGot,
        OrdertGetFailed,
        InventoryUpdated,
        InventoryUpdateFailed,
        InventoryRollback
    }
}
