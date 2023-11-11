namespace Saga.Orchestrator.OrderManager
{
    public enum EOrderTransactionState
    {
        NotStarted,
        BasketGot,
        BasketGetFailed,
        BasketDeleted,
        OrderCreated,
        OrderCreatedFailed,
        OrderDeleted,
        OrderDeletedFailed,
        OrderGot,
        OrdertGetFailed,
        InventoryUpdated,
        InventoryUpdateFailed,
        RollbackInventory,
        InventoryRollback,
        InventoryRollbackFailed
    }
}
