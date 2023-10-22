using Shared.Enums.Inventory;

namespace Shared.DTO.Inventory
{
    public record SalesProductDto(string ExternalDocumentNo, int Quantity)
    {
        public EDocumentType DocumentType = EDocumentType.Sales;
        public string ItemNo { get; set; }

        public void SetItemNo(string itemNo)
        {
            ItemNo = itemNo;
        }
    }
}
