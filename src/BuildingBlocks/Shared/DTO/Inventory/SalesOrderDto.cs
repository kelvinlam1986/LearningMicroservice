namespace Shared.DTO.Inventory
{
    public class SalesOrderDto
    {
        public string OrderNo { get; set; }

        public List<SalesItemDto> SalesItems { get; set; } = new List<SalesItemDto>();
    }
}
