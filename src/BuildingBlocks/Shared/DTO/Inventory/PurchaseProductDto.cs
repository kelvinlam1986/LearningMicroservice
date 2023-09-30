using Shared.Enums.Inventory;

namespace Shared.DTO.Inventory
{
    public class PurchaseProductDto
    {
        public EDocumentType DocumentType { get; set; } = EDocumentType.Purchase;

        public string? ItemNo { get; set; }

        public string? DocumentNo { get; set; }

        public string? ExtenalDocumentNo { get; set; }

        public int Quantity { get; set; }
    }
}
