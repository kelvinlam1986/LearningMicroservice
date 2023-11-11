using Shared.Enums.Inventory;

namespace Shared.DTO.Inventory
{
    public class SalesItemDto
    {
        public string ItemNo { get; set; }
        public int Quantity { get; set; }

        public EDocumentType DocumentType => EDocumentType.Sales;
    }
}
